using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motorino : MonoBehaviour
{
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    float speed, currentSpeed;
    float rotate, currentRotate;

    [Header("Parametri")]
    public float acceleration = 30f;
    public float steering = 80f;
    public float maxSteering = 30f;
    public float gravity = 10f;

    [Header("Parti motorino")]
    public Transform frontWheel;
    public Transform backWheel;
    public Transform steeringPart;


    private void Start()
    {
        
    }

    private void Update()
    {
        int dir = 0;

        //Per seguire la sfera
        transform.position = sphere.transform.position - new Vector3(0, 0.53f, .3f);

        speed = acceleration;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
            {
                dir = touch.position.x > Screen.width / 2 ? 1 : -1;
                Steer(dir);
            }else if (touch.phase == TouchPhase.Ended)
            {
                Steer(0);
            }
        }
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Animation
        kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + dir * steering, kartModel.localEulerAngles.z), .2f);
        /*if (amount > maxSteering * 2 / 3)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + dir * amount, 90 - (dir * amount)/3), .2f);
        }
        else
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + dir * amount, 90), .2f);
        }*/

        //steeringPart.localEulerAngles = new Vector3(0,(dir*amount)/3, -15.4f);
        
    }

    private void FixedUpdate()
    {
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f);

        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);

        //Accelerazione frontale
        if(hitOn.collider != null)
            sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravità
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Sterzare
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 2f);

    }

    public void Steer(int direction)
    {
        //if (amount > 30) amount = 30f;
        rotate = (steering * direction);
    }

}