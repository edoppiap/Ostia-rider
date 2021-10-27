using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCarController : MonoBehaviour
{
    public float movementSpeed = 7f;
    public float rotationSpeed = 75f;
    public float stopDistance = 5f;
    public bool reachedDestination;
    public bool touched = false;
    public LayerMask carLayer;
    public LayerMask playerLayer;

    private Vector3 velocity;
    public Vector3 destination;
    private Vector3 lastPosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Ground") && !collision.transform.CompareTag("Objects"))
        {
            touched = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f, carLayer | playerLayer);
        if (raycastHit)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10f, Color.white);
        }

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
