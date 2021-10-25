using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
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
        int count = 0;
        while(count < carToSpawn)
        {
            GameObject obj = Instantiate(carPrefab);
            int temp;
            do
            {
                temp = Random.Range(0, transform.childCount - 1);
            } while (alreadyUsedForSpawn.Contains(temp));
            alreadyUsedForSpawn.Add(temp);

            Transform child = transform.GetChild(temp);
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.position;
            obj.transform.forward = -child.transform.forward;

            yield return new WaitForEndOfFrame();

            count++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
