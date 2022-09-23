using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCarController : MonoBehaviour
{
    public float movementSpeed = 7f;
    public float rotationSpeed = 75f;
    public float stopDistance = 3f;
    public bool reachedDestination;
    public bool touched = false;
    public LayerMask carLayer;
    public LayerMask playerLayer;

    private Vector3 velocity;
    public Vector3 destination;
    private Vector3 lastPosition;
    public bool move = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Ground") && !collision.transform.CompareTag("Objects"))
        {
            touched = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitFwd, hitLeft, hitRight;
        bool raycastHitFwd = Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), out hitFwd, 10f, carLayer | playerLayer, QueryTriggerInteraction.Ignore);
        //bool raycastHitLeft = Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection((Vector3.forward - Vector3.right).normalized), out hitLeft, 10f, carLayer | playerLayer);
        bool raycastHitRight = Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection((Vector3.forward + Vector3.right).normalized), out hitRight, 10f, carLayer | playerLayer, QueryTriggerInteraction.Ignore);

        if (raycastHitFwd)
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * hitFwd.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * 10f, Color.white);
        }

        /*if (raycastHitLeft)
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection((Vector3.forward - Vector3.right).normalized) * hitLeft.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection((Vector3.forward - Vector3.right).normalized) * 10f, Color.white);
        }*/

        if (raycastHitRight)
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection((Vector3.forward + Vector3.right).normalized) * hitRight.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection((Vector3.forward + Vector3.right).normalized) * 10f, Color.white);
        }

        bool raycastHit = raycastHitFwd || raycastHitRight;

        if (!raycastHit &&
            transform.position != destination &&
            !touched)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if(destinationDistance >= stopDistance)
            {
                reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }


            velocity = (transform.position - lastPosition) / Time.deltaTime;
            velocity.y = 0;
            //var velocityMagnitude = velocity.magnitude;
            velocity = velocity.normalized;
            //var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            //var rightDotProduct = Vector3.Dot(transform.right, velocity);
            
        }
        lastPosition = transform.position;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
}
