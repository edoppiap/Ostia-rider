using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionBikeController : MonoBehaviour
{
    [Header("Collider part")]
    public Rigidbody bodyRb;
    //public Rigidbody frontSmallWheelRbDx;
    //public Rigidbody backSmallWheelRbDx;
    //public Rigidbody frontSmallWheelRbSx;
    //public Rigidbody backSmallWheelRbSx;
    public Rigidbody frontWheelRb;
    public Rigidbody backWheelRb;

    [Header("Transform part")]
    public Transform bodyTransform;
    public Transform frontWheelTransform;
    public Transform backWheelTransform;

    [Header("Parameters")]
    public float accellerationForce = 1500f;
    public float maxSpeed = 50f;
    public float turnVelocity = 50f;
    public float lateralForce = 50f;
    public float recenterTorque = 5f;
    public float torqueStabilizer = 50f;
    public LayerMask groundLayer;
    public float applicationDeltaPoint = -1.2f;

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    private GameManager gameManager;
    private Vector3 lateralVelocity = Vector3.zero;
    private bool isGrounded;

    //accelleration parameters
    private float accelleration;
    private float lastVelocity = 0;

    void CalculateAccelleration()
    {
        float velocity = Vector3.Dot(bodyRb.transform.forward, bodyRb.velocity);
        accelleration = (velocity - lastVelocity) / Time.deltaTime;
        lastVelocity = velocity;
    }

    public float GetAccelleration()
    {
        return accelleration;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

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
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        frontWheelTransform.position = frontWheelRb.transform.position;
        frontWheelTransform.rotation = frontWheelRb.transform.rotation;

        backWheelTransform.position = backWheelRb.transform.position;
        backWheelTransform.rotation = backWheelRb.transform.rotation;

        bodyTransform.position = bodyRb.transform.position + (Vector3.up * - 0.283f);
        bodyTransform.rotation = bodyRb.transform.rotation;    

        //raycast ground check
        RaycastHit hit;
        isGrounded = Physics.Raycast(bodyRb.position, -bodyRb.transform.up, out hit, 1f, groundLayer);

        if (isGrounded)
        {
            Debug.DrawRay(bodyRb.position, -bodyRb.transform.up * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(bodyRb.position, -bodyRb.transform.up * 1f, Color.white);
        }

        //rotate the car parallel to the ground
        //rotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        //modifica l'attrito in base a se l'RB è a terra
        //bodyRB.drag = isGrounded ? normalDrag : lowDrag;     

    }

    void Stabilizer()
    {
        Vector3 axisFromRotate = Vector3.Cross(bodyRb.transform.up, Vector3.up);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * torqueStabilizer;
        //torqueForce.x *= .4f;
        torqueForce -= bodyRb.angularVelocity;
        bodyRb.AddTorque(torqueForce * bodyRb.mass * .02f, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (gameManager.IsInPlay())
        {
            Vector3 accelleration = Vector3.zero;
            Vector3 torque = Vector3.zero;
            if (isGrounded)
            {
                bodyRb.AddForceAtPosition(bodyRb.transform.forward * accellerationForce * customVerticalAxis, 
                    bodyRb.transform.position + (bodyRb.transform.up * applicationDeltaPoint));
                torque = bodyRb.transform.up * turnVelocity * customHorizontalAxis;
            }
            else
            {
                bodyRb.AddForce(-Vector3.up * 100f);
            }

            bodyRb.AddTorque(torque);

            bodyRb.velocity = Vector3.ClampMagnitude(bodyRb.velocity, maxSpeed);

            lateralVelocity = Vector3.Dot(bodyRb.transform.right, bodyRb.velocity) * bodyRb.transform.right;

            if (lateralVelocity.magnitude > 0)
            {
                bodyRb.AddForce(-lateralVelocity * lateralForce * bodyRb.mass);
            }

        }
        Stabilizer();
        CalculateAccelleration();

        {

            /*if (gameManager.IsInPlay())
            {
                if (isGrounded)
                {
                    Vector3 force = transform.forward * accelleration;
                    Vector3 applicationPos = transform.position - (.3f*Vector3.up); //dovrebbe spostare in basso il punto di applicazione
                    bodyRB.AddForceAtPosition(Vector3.Lerp(-force, force, accelerationInterpolation), applicationPos, ForceMode.Acceleration); //accellera il RB interpolando 
                }
                else
                {
                    Vector3 gravity = transform.up * -50f;
                    bodyRB.AddRelativeForce(gravity);
                }

                float velocity = bodyRB.velocity.x;
                if (Mathf.Abs(velocity) > 0)
                {
                    bodyRB.AddRelativeForce(new Vector3(-velocity * lateralForce, 0, 0));
                }

                Vector3 relativeTorque = transform.up * customHorizontalAxis * turnVelocity; //formula che aggiunge la rotazione al RB
                relativeTorque = accelerationInterpolation < .45f ? -relativeTorque : relativeTorque; //inverte lo sterzo se va in retromarcia
                bodyRB.AddRelativeTorque(relativeTorque, ForceMode.Acceleration);
                bodyRB.MoveRotation(Quaternion.Slerp(transform.rotation, rotateTo, alignToGroungTime * Time.deltaTime).normalized); //ruota l'RB in base alla normale del suolo
            }*/

        }


    }
}
