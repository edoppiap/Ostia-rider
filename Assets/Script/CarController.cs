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

    private float moveInputLearped;
    private float timePassed = 0;

    [Header("Drag")]
    public float airDrag;
    private float normalDrag;

    [Header("Parametri")]
    public float fwdSpeed;
    public float brakeSensitivity;
    public float revSpeed;
    public float turnSpeed;
    public float alignToGroungTime = 5;
    public LayerMask groundLayer;
    
    [Header("RigidBody")]
    public Rigidbody sphereRB;
    public Rigidbody colliderRB;

    public void SterzaDx()
    {
        turnInput = moveInput != 0 ? 1 : 0;
    }

    public void SterzaSx()
    {
        turnInput = moveInput != 0 ? -1 : 0;
    }

    public void DeSterza()
    {
        turnInput = 0;
    }

    public void Accellera()
    {
        sphereRB.drag = 4f;
        moveInput = fwdSpeed;
    }

    public void Decellera()
    {
        sphereRB.drag = .01f;
        moveInput = 0;
    }

    public void Frena()
    {
        moveInput = -revSpeed;
    }

    public void DeFrena()
    {
        moveInput = 0;
    }

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
        if(moveInput != 0)
            timePassed += Time.deltaTime;
        else
            timePassed = 0;
        moveInputLearped = Mathf.Lerp(0, moveInput, timePassed);
        
        //seguire la sfera
        transform.position = sphereRB.transform.position;
        //sterzare
        float newRotation = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(0, newRotation, 0, Space.World);

        //raycast ground check
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f,  groundLayer);

        //rotate the car parallel to the ground
        Quaternion rotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, alignToGroungTime * Time.deltaTime);

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
            sphereRB.AddForce(transform.forward * moveInputLearped, ForceMode.Acceleration); //muove la macchina
        else
            sphereRB.AddForce(transform.up * -50f); //aggiunge la gravità

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
