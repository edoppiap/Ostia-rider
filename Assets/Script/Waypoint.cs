using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;
    public bool usableForSpawn = true;

    [Range(0f, 5f)]
    public float width = 1f;

    public List<Waypoint> branches = new List<Waypoint>();

    [Range(0f, 1f)]
    public float branchRatio = .5f;

    private BoxCollider addedCollider;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Objects"))
            usableForSpawn = false;
    }

    private void OnTriggerExit(Collider other)
    {
        usableForSpawn = true;
    }

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 2f;
        Vector3 maxBound = transform.position - transform.right * width / 2f;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }

    private void Start()
    {
        addedCollider = gameObject.AddComponent<BoxCollider>();
        addedCollider.size = new Vector3(8,1,8);
        addedCollider.isTrigger = true;
        addedCollider.center = Vector3.up;

        gameObject.layer = 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
