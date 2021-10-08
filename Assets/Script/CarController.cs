using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    //get input
    //use input to move sphere
    //set cars position to sphere

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;
    private float lastVelocity = 0;
    private float brakeTime = 0;
    private float accelationTime = 0;

    [Header("Drag")]
    public float airDrag;
    private float normalDrag;

    [Header("Parametri")]
    public float fwdSpeed;
    public float brakeSensitivity;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;
    
    [Header("RigidBody")]
    public Rigidbody sphereRB;
    public Rigidbody colliderRB;

    // Start is called before the first frame update
    void Start()
    {
        sphereRB.transform.parent = null;
        colliderRB.transform.parent = null;
        normalDrag = sphereRB.drag;
    }

    // Update is called once per frame
    void Update()
    {
        //decidere la velocità
        if(Input.touchCount == 2)
        {
            //moveInput = -revSpeed;
            brakeTime += Time.deltaTime;
            if (moveInput > 0)
                moveInput = Mathf.Lerp(fwdSpeed, 0, brakeTime / brakeSensitivity);
            else
                moveInput = -revSpeed;
            //moveInput = Mathf.Lerp(fwdSpeed, -revSpeed, brakeTime / brakeSensitivity);
            accelationTime = 0;
        }
        else
        {
            accelationTime += Time.deltaTime;
            moveInput = Mathf.Lerp(0, fwdSpeed, accelationTime);
            brakeTime = 0;
        }
        //decidere se sterzare

        if(Input.touchCount == 1)
            turnInput = Input.GetTouch(0).position.x > Screen.width / 2 ? 1 : -1;
        else
            turnInput = 0;

        //seguire la sfera
        transform.position = sphereRB.transform.position;
        //sterzare
        float newRotation = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(0, newRotation, 0, Space.World);

        //raycast ground check
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f,  groundLayer);

        //rotate the car parallel to the ground
        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        //aggiusta l'attrito in base a se è a terra o no
        sphereRB.drag = isCarGrounded ? normalDrag : airDrag;

    }

    public float GetMoveInput()
    {
        return moveInput;
    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration); //muove la macchina
        else
            sphereRB.AddForce(transform.up * -9.8f); //aggiunge la gravità

        colliderRB.MoveRotation(transform.rotation);
    }

    public bool isGrounded()
    {
        return isCarGrounded;
    }

    public bool isAccellerating()
    {
        float accelleration = (sphereRB.velocity.z - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = sphereRB.velocity.z;
        float acc = Mathf.Abs(Mathf.Round(accelleration));


        return !(acc < 15);
    }
}
