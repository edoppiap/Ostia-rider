using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject autoParent;
    public GameObject[] carPrefab;
    public int carToSpawn;

    private bool spawned = false;
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
        foreach (Transform t in transform)
        {
            availableForSpawn.Add(t.gameObject);
        }

        while (autoParent.transform.childCount < carToSpawn)
        {
            GameObject obj = Instantiate(carPrefab[UnityEngine.Random.Range(0, carPrefab.Length)]);
            AssignWaypoint(obj);
            /*
            Transform tempWaypointTransform;
            //Vector2 inCameraPosition;
            //bool inCameraBool;
            int temp;
            do
            {
                temp = Random.Range(0, transform.childCount - 1);
                tempWaypointTransform = transform.GetChild(temp);

                //inCameraPosition = Camera.main.WorldToViewportPoint(tempWaypointTransform.transform.position);
                //inCameraBool = inCameraPosition.x > 0 && inCameraPosition.x < 1 && inCameraPosition.y > 0 && inCameraPosition.y < 1;
                //} while (alreadyUsedForSpawn.Contains(temp) && !transform.GetChild(temp).GetComponent<Waypoint>().usableForSpawn);
            } while (alreadyUsedForSpawn.Contains(temp) && !tempWaypointTransform.GetComponent<Waypoint>().usableForSpawn);
            alreadyUsedForSpawn.Add(temp);

            obj.GetComponent<WaypointNavigator>().currentWaypoint = tempWaypointTransform.GetComponent<Waypoint>();
            obj.transform.position = tempWaypointTransform.position + Vector3.up;
            obj.transform.forward = -tempWaypointTransform.transform.forward; */

            obj.GetComponentInChildren<Despawn>().SetCarSpawner(this);
            obj.transform.SetParent(autoParent.transform);

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
        foreach (Transform t in transform)
        {
            availableForSpawn.Add(t.gameObject);
        }
        foreach (GameObject obj in disabledCarList.ToArray())
        {
            AssignWaypoint(obj);
            obj.SetActive(true);
            disabledCarList.Remove(obj);
        }
        availableForSpawn.Clear();
    }

    void AssignWaypoint(GameObject obj)
    {
        Waypoint way = null;
        int temp;
        do
        {
            if (availableForSpawn.Count == 0)
            {
                temp = -1;
                break;
            }
            temp = Random.Range(0, availableForSpawn.Count);
            way = availableForSpawn[temp].GetComponent<Waypoint>();

            //temp = Random.Range(0, transform.childCount - 1);
            //way = transform.GetChild(temp);
            if (!way.usableForSpawn || (way.nextWaypoint != null && way.nextWaypoint.deSpawn))
                availableForSpawn.RemoveAt(temp);

        } while (!way.usableForSpawn || (way.nextWaypoint != null && way.nextWaypoint.deSpawn));
        
        if(temp != -1)
            availableForSpawn.RemoveAt(temp);

        obj.GetComponent<WaypointNavigator>().SetCarSpawner(this);
        if (way != null)
        {
            obj.GetComponent<WaypointNavigator>().currentWaypoint = way;
            obj.GetComponent<TrafficCarController>().SetDestination(way.GetPosition());
            obj.transform.position = way.transform.position;
            obj.transform.forward = -way.transform.forward;
        }
        else
        {
            obj.GetComponent<WaypointNavigator>().Despawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawned && disabledCarList.Count > 0)
            ReEnable();
    }
}
