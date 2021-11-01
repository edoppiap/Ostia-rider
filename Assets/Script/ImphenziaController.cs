using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImphenziaController : MonoBehaviour
{

    public WheelCollider frontWheelColliderOne;
    public WheelCollider backWheelCollider;

    public Transform frontWheelTransform;
    public Transform backWheelTransform;
    public Transform parafanghiDavanti;
    public Transform manubrio;

    public float motorTorque = 100f;
    public float maxSteer = 20f;

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;

    public void SterzaDx()
    {
        customHorizontalAxis = 1;
    }

    public void SterzaSx()
    {
        customHorizontalAxis = -1;
    }

    public void Desterza()
    {
        customHorizontalAxis = 0;
    }

    public void Accellera()
    {
        customVerticalAxis = 1;
    }

    public void Decellera()
    {
        customVerticalAxis = 0;
    }

    public void Frena()
    {
        customVerticalAxis = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pos = Vector3.zero;
        var rot = Quaternion.identity;


        frontWheelColliderOne.GetWorldPose(out pos, out rot);
        frontWheelTransform.position = pos;
        frontWheelTransform.rotation =  rot;

        //parafanghiDavanti.rotation *= Quaternion.Euler(0, rot.eulerAngles.y, 0);
        //manubrio.rotation *= Quaternion.Euler(0, rot.eulerAngles.y, 0);

        backWheelCollider.GetWorldPose(out pos, out rot);
        backWheelTransform.position = pos;
        backWheelTransform.rotation = rot;
    }

    private void FixedUpdate()
    {
        backWheelCollider.motorTorque = customVerticalAxis * motorTorque;
        frontWheelColliderOne.steerAngle = customHorizontalAxis * maxSteer;


    }
}
