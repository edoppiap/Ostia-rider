using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAndRotate : MonoBehaviour
{
    public bool bounce;
    public Vector3 rotate = new Vector3();
    public float degrees = 360f;
    public float time = 1f;

    private void Start()
    {
        if (bounce)
            LeanTween.moveY(gameObject, 10f, 0.3f).setLoopPingPong();
        if(degrees == 360f)
            LeanTween.rotateAround(gameObject, rotate, degrees, time).setLoopClamp();
        else
            LeanTween.rotateAround(gameObject, rotate, degrees, time).setLoopPingPong();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
