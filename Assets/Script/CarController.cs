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
    //private float turnInput;
    private bool isCarGrounded;
    private float lastVelocity = 0;

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    //private float moveInputLearped;
    //private float timePassed = 0;

    private GameManager gameManager;

    [Header("Drag")]
    public float airDrag;
    private float normalDrag;

    [Header("Parametri")]
    public float fwdSpeed;
    public float brakeSensitivity;
    public float revSpeed;
    public float turnSpeed;
    public float lateralForce = 50f;
    public float maxSpeed = 30f;
    public float alignToGroungTime = 5;
    public LayerMask groundLayer;
    
    [Header("RigidBody")]
    public Rigidbody sphereRB;
    public Transform colliderTransform;

    public void SterzaDx()
    {
        customHorizontalAxis = 1;
        //turnInput = moveInput != 0 ? 1 : 0;
    }

    public void SterzaSx()
    {
        customHorizontalAxis = -1;
        //turnInput = moveInput != 0 ? -1 : 0;
    }

    public void DeSterza()
    {
        customHorizontalAxis = 0;
        //turnInput = 0;
    }

    public void Accellera()
    {
        customVerticalAxis = 1;
        //sphereRB.drag = 4f;
        //moveInput = fwdSpeed;
    }

    public void Decellera()
    {
        customVerticalAxis = 0;
        //sphereRB.drag = .01f;
        //moveInput = 0;
    }

    public void Frena()
    {
        customVerticalAxis = -1;
        //moveInput = revSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        sphereRB.transform.parent = null;
        //colliderRB.transform.parent = null;
        //normalDrag = sphereRB.drag;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(moveInput != 0)
            timePassed += Time.deltaTime;
        else
            timePassed = 0;
        moveInputLearped = Mathf.Lerp(0, moveInput, timePassed);*/
        
        //seguire la sfera
        transform.position = sphereRB.transform.position;
        transform.rotation = sphereRB.transform.rotation;
        colliderTransform.position = sphereRB.transform.position;
        colliderTransform.rotation = sphereRB.transform.rotation;
        //sterzare
        //float newRotation = turnInput * turnSpeed * Time.deltaTime * customVerticalAxis;
        //transform.Rotate(0, newRotation, 0, Space.World);

        //raycast ground check
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f,  groundLayer);

        //rotate the car parallel to the ground
        Quaternion rotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, alignToGroungTime * Time.deltaTime);

        //aggiusta l'attrito in base a se è a terra o no
        //sphereRB.drag = isCarGrounded ? normalDrag : airDrag;

    }

    public float GetMoveInput()
    {
        return moveInput;
    }

    private void FixedUpdate()
    {
        if (gameManager.IsInPlay())
        {
            if (isCarGrounded)
            {
                Vector3 fwdForce = sphereRB.transform.forward * fwdSpeed * customVerticalAxis;
                Vector3 ltForce = Vector3.zero;
                //sphereRB.AddForce(sphereRB.transform.forward * moveInput * customVerticalAxis); //muove la macchina

                Vector3 lateralVelocity = Vector3.Dot(sphereRB.transform.right, sphereRB.velocity) * sphereRB.transform.right;
                //float lateralAccelleration = lateralVelocity.magnitude;
                if (lateralVelocity.magnitude > 0)
                {
                    ltForce = -lateralVelocity * lateralForce * sphereRB.mass;
                    //sphereRB.AddForce(-lateralDownforce * lateralVelocity.magnitude * lateralForce * sphereRB.mass);
                }

                sphereRB.AddForce(fwdForce + ltForce);
                sphereRB.AddTorque(sphereRB.transform.up * turnSpeed * customHorizontalAxis);
            }
            //else
            //sphereRB.AddForce(transform.up * -50f); //aggiunge la gravità

            sphereRB.velocity = Vector3.ClampMagnitude(sphereRB.velocity, maxSpeed);


            //colliderTransform.MoveRotation(transform.rotation);
            //colliderTransform.MovePosition(transform.position);
        }
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
