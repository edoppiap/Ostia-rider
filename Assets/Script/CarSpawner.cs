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
    }

    IEnumerator Spawn()
    {
        while(parent.transform.childCount < carToSpawn)
        {
            GameObject obj = Instantiate(carPrefab[Random.Range(0, carPrefab.Length)]);
            Transform tempWaypointTransform;
            //Vector2 inCameraPosition;
            //bool inCameraBool;
            int temp;
            do
            {
                temp = Random.Range(0, transform.childCount - 1);
                tempWaypointTransform = transform.GetChild(temp);

                if (alreadyUsedForSpawn.Contains(temp))
                    Debug.Log("Cioccato "+tempWaypointTransform.ToString());
                
                //inCameraPosition = Camera.main.WorldToViewportPoint(tempWaypointTransform.transform.position);
                //inCameraBool = inCameraPosition.x > 0 && inCameraPosition.x < 1 && inCameraPosition.y > 0 && inCameraPosition.y < 1;
                //} while (alreadyUsedForSpawn.Contains(temp) && !transform.GetChild(temp).GetComponent<Waypoint>().usableForSpawn);
            } while (alreadyUsedForSpawn.Contains(temp) && !tempWaypointTransform.GetComponent<Waypoint>().usableForSpawn);
            alreadyUsedForSpawn.Add(temp);

            obj.GetComponent<WaypointNavigator>().currentWaypoint = tempWaypointTransform.GetComponent<Waypoint>();
            obj.transform.position = tempWaypointTransform.position+ Vector3.up;
            obj.transform.forward = -tempWaypointTransform.transform.forward;
            obj.transform.SetParent(parent.transform);

            yield return new WaitForEndOfFrame();
        }
        alreadyUsedForSpawn.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (parent.transform.childCount < carToSpawn)
            StartCoroutine(Spawn());
        
    }
}
