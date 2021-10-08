using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAndRotate : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        LeanTween.moveY(gameObject, 10f, 0.3f).setLoopPingPong();
        LeanTween.rotateAround(gameObject, Vector3.up, 360f, 1f).setLoopClamp();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
