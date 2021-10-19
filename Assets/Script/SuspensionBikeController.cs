using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionBikeController : MonoBehaviour
{
    public float accelleration = 50f;
    public float turnVelocity = 50f;
    public float lateralForce = 50f;
    public LayerMask groundLayer;
    public float alignToGroungTime = 5f;

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    private Rigidbody bodyRB;
    private GameManager gameManager;
    private bool isGrounded;
    private Quaternion rotateTo;
    private int isReversed = 1;

    public void Accellera()
    {
        isReversed = 1;
        customVerticalAxis = 1;
    }

    public void Deaccellera()
    {
        customVerticalAxis = 0;
    }

    public void Frena()
    {
        isReversed = -1;
        customVerticalAxis = -1;
    }

    public void SterzaDx()
    {
        isReversed = 1;
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
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //raycast ground check
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);
        //isGrounded = Physics.Raycast(transform.position, -transform.up, 1f, groundLayer);

        //rotate the car parallel to the ground
        rotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, alignToGroungTime * Time.deltaTime);

        bodyRB.drag = isGrounded ? 5f : .5f;     

    }

    private void FixedUpdate()
    {
        if (gameManager.IsInPlay() && gameManager.GetTimeRemaining() > 0)
        {
            if (isGrounded)
            {
                Vector3 force = transform.forward * customVerticalAxis * accelleration;
                Vector3 applicationPos = transform.position - (Vector3.up);
                bodyRB.AddForceAtPosition(force, applicationPos); //accellera la macchina
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
            
            Vector3 relativeTorque = transform.up * customHorizontalAxis * turnVelocity * isReversed;
            bodyRB.AddRelativeTorque(relativeTorque);
            bodyRB.MoveRotation(Quaternion.Slerp(transform.rotation, rotateTo, alignToGroungTime * Time.deltaTime).normalized);
        }
    }
}
