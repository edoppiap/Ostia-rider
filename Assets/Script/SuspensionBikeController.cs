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
    public float normalDrag = 5f;
    public float lowDrag = .5f;

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    private Rigidbody bodyRB;
    private GameManager gameManager;
    private bool isGrounded;
    private Quaternion rotateTo;
    private float accelerationInterpolation = .5f;

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

    //modifica il valore dell'interpolazione dell'accellerazione per dare l'effetto dell'inerzia
    void AccellerationInterpolationModify()
    {
        if (customVerticalAxis != 0 && isGrounded)
        {
            if (customVerticalAxis == 1 && accelerationInterpolation < 1f)
                accelerationInterpolation += Time.deltaTime;
            if (customVerticalAxis == -1 && accelerationInterpolation > 0f)
                accelerationInterpolation -= Time.deltaTime;
        }
        else
        {
            if (accelerationInterpolation > .5f)
            {
                accelerationInterpolation -= Time.deltaTime / 10;
                if (accelerationInterpolation < .5f)
                    accelerationInterpolation = .5f;
            }
            else if (accelerationInterpolation < .5f)
            {
                accelerationInterpolation += Time.deltaTime / 10;
                if (accelerationInterpolation > .5f)
                    accelerationInterpolation = .5f;
            }
        }
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
        AccellerationInterpolationModify();        

        //raycast ground check
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        //rotate the car parallel to the ground
        rotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        //modifica l'attrito in base a se l'RB è a terra
        bodyRB.drag = isGrounded ? normalDrag : lowDrag;     

    }

    private void FixedUpdate()
    {
        if (gameManager.IsInPlay())
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
        }
    }
}
