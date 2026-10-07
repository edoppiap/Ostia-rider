using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject autoParent;
    public GameObject[] carPrefab;
    public int carToSpawn;
    [Tooltip("Ogni quanti secondi si riprova a piazzare le auto disattivate")]
    public float retryInterval = .5f;

    private bool spawned = false;
    private float retryTimer = 0f;
    private List<GameObject> availableForSpawn = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> disabledCarList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        FillAvailableForSpawn();

        while (autoParent.transform.childCount < carToSpawn)
        {
            GameObject prefab = carPrefab[Random.Range(0, carPrefab.Length)];
            Waypoint way = TakeSpawnWaypoint();
            GameObject obj = way != null
                ? Instantiate(prefab, way.transform.position, Quaternion.LookRotation(-way.transform.forward), autoParent.transform)
                : Instantiate(prefab, autoParent.transform);
            obj.GetComponentInChildren<Despawn>().SetCarSpawner(this);
            obj.GetComponent<WaypointNavigator>().SetCarSpawner(this);

            if (way != null)
                PlaceOnWaypoint(obj, way);
            else
                DisableCar(obj); //nessun waypoint libero: verrà piazzata più tardi da ReEnable

            yield return new WaitForEndOfFrame();
        }
        availableForSpawn.Clear();
        spawned = true;
    }

    public void DisableCar(GameObject obj)
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
        obj.GetComponent<TrafficCarController>().touched = false;
        disabledCarList.Add(obj);
    }

    void ReEnable()
    {
        FillAvailableForSpawn();
        foreach (GameObject obj in disabledCarList.ToArray())
        {
            Waypoint way = TakeSpawnWaypoint();
            if (way == null)
                break; //riprova al prossimo giro

            obj.SetActive(true); //prima di spostarla, così il Rigidbody esiste già e viene spostato anche lui
            PlaceOnWaypoint(obj, way);
            disabledCarList.Remove(obj);
        }
        availableForSpawn.Clear();
    }

    void FillAvailableForSpawn()
    {
        availableForSpawn.Clear();
        foreach (Transform t in transform)
        {
            availableForSpawn.Add(t.gameObject);
        }
    }

    //estrae un waypoint casuale libero (ognuno al massimo una volta per giro), null se non ce ne sono
    Waypoint TakeSpawnWaypoint()
    {
        while (availableForSpawn.Count > 0)
        {
            int i = Random.Range(0, availableForSpawn.Count);
            Waypoint way = availableForSpawn[i].GetComponent<Waypoint>();
            availableForSpawn.RemoveAt(i);

            if (way.usableForSpawn && (way.nextWaypoint == null || !way.nextWaypoint.deSpawn))
                return way;
        }
        return null;
    }

    void PlaceOnWaypoint(GameObject obj, Waypoint way)
    {
        obj.GetComponent<WaypointNavigator>().currentWaypoint = way;
        TrafficCarController controller = obj.GetComponent<TrafficCarController>();
        controller.Teleport(way.transform.position, Quaternion.LookRotation(-way.transform.forward));
        controller.SetDestination(way.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned || disabledCarList.Count == 0)
            return;

        retryTimer -= Time.deltaTime;
        if (retryTimer <= 0f)
        {
            retryTimer = retryInterval;
            ReEnable();
        }
    }
}
