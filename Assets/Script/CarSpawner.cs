using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject parent;
    public GameObject[] carPrefab;
    public int carToSpawn;

    private List<int> alreadyUsedForSpawn;

    // Start is called before the first frame update
    void Start()
    {
        alreadyUsedForSpawn = new List<int>();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(parent.transform.childCount < carToSpawn)
        {
            GameObject obj = Instantiate(carPrefab[Random.Range(0, carPrefab.Length)]);
            int temp;
            do
            {
                temp = Random.Range(0, transform.childCount - 1);
            } while (alreadyUsedForSpawn.Contains(temp) && !transform.GetChild(temp).GetComponent<Waypoint>().usableForSpawn);
            alreadyUsedForSpawn.Add(temp);

            Transform child = transform.GetChild(temp);
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.position+ Vector3.up;
            obj.transform.forward = -child.transform.forward;
            obj.transform.SetParent(parent.transform);

            yield return new WaitForEndOfFrame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (parent.transform.childCount < carToSpawn)
            StartCoroutine(Spawn());
        
    }
}
