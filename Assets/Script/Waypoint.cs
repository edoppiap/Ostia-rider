using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;
    public bool deSpawn;
    public bool usableForSpawn;
    public LayerMask layerMask = 7 & 11;

    [Range(0f, 5f)]
    public float width = 1f;

    public List<Waypoint> branches = new List<Waypoint>();

    [Range(0f, 1f)]
    public float branchRatio = .5f;

    private Collider[] hitColliders;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 2f;
        Vector3 maxBound = transform.position - transform.right * width / 2f;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }

    private void Start()
    {
        usableForSpawn = true;

        gameObject.layer = 6;
    }

    // Update is called once per frame
    void Update()
    {
        hitColliders = Physics.OverlapSphere(transform.position, 10f, layerMask);
        if (hitColliders.Length > 0)
            usableForSpawn = false;
        else
            usableForSpawn = true;
    }
}
