using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }
}
