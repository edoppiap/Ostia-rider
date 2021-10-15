using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionBikeController : MonoBehaviour
{
    public float accelleration = 50f;
    public float turnVelocity = 50f;

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    private Rigidbody bodyRB;

    public void Accellera()
    {
        customVerticalAxis = 1;
    }

    public void Deaccellera()
    {
        customVerticalAxis = 0;
    }

    public void Frena()
    {
        customVerticalAxis = -1;
    }

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

    // Start is called before the first frame update
    void Start()
    {
        bodyRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        bodyRB.AddForce(Vector3.forward * customVerticalAxis * accelleration, ForceMode.Acceleration);
        bodyRB.AddTorque(transform.up * customHorizontalAxis * turnVelocity, ForceMode.Force);
    }
}
