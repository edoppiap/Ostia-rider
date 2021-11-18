using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{

    TrafficCarController controller;
    CarSpawner carSpawner;
    public Waypoint currentWaypoint;

    public void SetCarSpawner(CarSpawner spawner)
    {
        carSpawner = spawner;
    }

    public void Despawn()
    {
        carSpawner.DisableCar(controller.gameObject);
    }


    private void Awake()
    {
        controller = GetComponent<TrafficCarController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //controller.SetDestination(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.reachedDestination)
        {
            bool shouldBranch = false;

            if(currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio ? true : false;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
            }
            else
            {
                if(currentWaypoint.nextWaypoint != null)
                {
                    currentWaypoint = currentWaypoint.nextWaypoint;
                    controller.SetDestination(currentWaypoint.GetPosition());
                }
            }


            if (currentWaypoint.deSpawn)
            {
                Despawn();
            }

        }
    }
}
