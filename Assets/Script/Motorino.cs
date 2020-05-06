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
    Vector2 initialPosition;

    [Header("Parametri")]
    public float acceleration = 30f;
    public float steering = 80f;
    public float maxSteering = 30f;
    public float gravity = 10f;

    [Header("Parti motorino")]
    public Transform frontWhell;
    public Transform backWhell;


    private void Start()
    {
        
    }

    private void Update()
    {
        int dir = 0;
        float amount = 0;

        //Per seguire la sfera
        transform.position = sphere.transform.position - new Vector3(0, 0.73f, .3f);

        speed = acceleration;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                initialPosition = touch.position;
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                dir = initialPosition.x < touch.position.x ? 1 : -1;
                amount = Mathf.Abs(initialPosition.x - touch.position.x) / 2;
                if (amount > maxSteering) amount = maxSteering;
                Steer(dir, amount);
            }
        }
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Animation
        kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90+ dir * amount, kartModel.localEulerAngles.z), .2f);
    }

    private void FixedUpdate()
    {
        //Accelerazione frontale
        sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravità
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Sterzare
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f);

        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Steer(int direction, float amount)
    {
        //if (amount > 30) amount = 30f;
        rotate = (steering * direction) * amount;
    }

}