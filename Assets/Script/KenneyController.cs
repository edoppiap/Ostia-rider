using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KenneyController : MonoBehaviour
{
    [Header ("Collider & Rigidbody")]
    public Rigidbody sphereRB;
    public Collider capsuleCollider;

    [Header("Actual models")]
    public Transform motorbikeMesh;
    public Transform frontWheelMesh;
    public Transform backWheelMesh;

    [Header("Values")]
    public float accelleration;
    public float maxSpeed = 30f;
    public float steer;
    public LayerMask groundLayer = 9;

    //private values
    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        sphereRB.transform.parent = null;
        capsuleCollider.transform.parent = null;
    }

    public void Accelerate()
    {
        customVerticalAxis = 1;
    }

    public void DeAccelerate()
    {
        customVerticalAxis = 0;
    }

    public void Brake()
    {
        customVerticalAxis = -1;
    }

    public void RightSteer()
    {
        customHorizontalAxis = 1;
    }

    public void LeftSteer()
    {
        customHorizontalAxis = -1;
    }

    public void StopSteer()
    {
        customHorizontalAxis = 0;

    }

    // Update is called once per frame
    void Update()
    {
        motorbikeMesh.position = sphereRB.transform.position + (-Vector3.up * .22f);

        motorbikeMesh.Rotate(Vector3.up, steer * customHorizontalAxis * Time.deltaTime);

        capsuleCollider.transform.position = motorbikeMesh.position;
        capsuleCollider.transform.rotation = motorbikeMesh.rotation;

        //raycast ground check
        RaycastHit hit;
        isGrounded = Physics.Raycast(motorbikeMesh.position, -Vector3.up, out hit, 1f, groundLayer);

        if (isGrounded)
        {
            Debug.DrawRay(motorbikeMesh.position, -Vector3.up * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(motorbikeMesh.position, -Vector3.up * 1f, Color.white);
        }
    }

    private void FixedUpdate()
    {
        //fa roteare la sfera
        if (customVerticalAxis != 0)
            sphereRB.AddTorque(motorbikeMesh.right * accelleration * customVerticalAxis);
        //la velocità di rotazione di default è 7, molto bassa, quindi la modifico
        sphereRB.maxAngularVelocity = maxSpeed;
    }
}
