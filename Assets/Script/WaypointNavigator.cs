using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{

    TrafficCarController controller;
    public Waypoint currentWaypoint;


    private void Awake()
    {
        controller = GetComponent<TrafficCarController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller.SetDestination(currentWaypoint.GetPosition());
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
                currentWaypoint = currentWaypoint.nextWaypoint;
                controller.SetDestination(currentWaypoint.GetPosition());
            }

        }
    }
}
