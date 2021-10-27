using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    void OnBecameInvisible()
    {
        if (transform.parent.GetComponent<TrafficCarController>().touched)
                Destroy(transform.parent.gameObject);

    }
}
