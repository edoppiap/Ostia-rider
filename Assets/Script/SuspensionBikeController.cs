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
    //public Transform rig;

    [Header("Audio part")]
    public AudioClip starting;
    public AudioClip idle;
    public AudioClip accellerating;
    public AudioClip deaccellerating;
    public AudioClip topSpeed;
    [Range(0f,1f)]
    public float maxVolume=.8f;
    private AudioSource track01, track02;
    bool isPlayingTrack01;
    bool is_accellerating = false;
    bool is_decellerating = false;
    bool is_idle = false;
    bool is_topSpeed = false;

    [Header("Parameters")]
    public float accellerationForce = 1500f;
    public float maxSpeed = 50f;
    public float turnVelocity = 50f;
    public float lateralForce = 50f;
    public float recenterTorque = 5f;
    public float torqueStabilizer = 50f;
    public LayerMask groundLayer;
    public float applicationDeltaPoint = .7f;
    public float differentUpPosition = .39f;
    /*public float differentRigUpPosition = .66f;
    public float differentRigFwdPosition = .23f;
    public float differentRigRightPosition = -.12f;*/

    private int customVerticalAxis = 0;
    private int customHorizontalAxis = 0;
    private GameManager gameManager;
    private Vector3 lateralVelocity = Vector3.zero;
    private bool isGrounded;

    //accelleration parameters
    public float accelleration;
    public float velocity;
    private float lastVelocity = 0;

    public float GetCustomHorizontal()
    {
        return customHorizontalAxis;
    }

    public float GetAccellerationForce()
    {
        return accellerationForce * customVerticalAxis;
    }

    void CalculateAccelleration()
    {
        velocity = Vector3.Dot(bodyRb.transform.forward, bodyRb.velocity);
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
        if(velocity > -2)
        {
            SwapTrack(accellerating);
        }
        else
        {
            SwapTrack(deaccellerating);
        }
        if (isPlayingTrack01)
            track01.loop = false;
        else
            track02.loop = false;
    }

    public void Deaccellera()
    {
        customVerticalAxis = 0;
        if(Mathf.Abs(velocity) > 10)
        {
            SwapTrack(deaccellerating);
            if (isPlayingTrack01)
                track01.loop = false;
            else
                track02.loop = false;
        }
        else
        {
            SwapTrack(idle);

            if (isPlayingTrack01)
                track01.loop = true;
            else
                track02.loop = true;
        }
    }

    public void Frena()
    {
        customVerticalAxis = -1;
        if(velocity < -2)
        {
            SwapTrack(accellerating);
        }
        else
        {
            SwapTrack(deaccellerating);
        }
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

    public void SwapTrack(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));
        isPlayingTrack01 = !isPlayingTrack01;
    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float timeToFade = 0.5f;
        float timeElapsed = 0f;
        if (isPlayingTrack01)
        {
            track02.clip = newClip;
            track02.Play();

            while(timeElapsed < timeToFade)
            {
                track02.volume = Mathf.Lerp(0, maxVolume, timeElapsed / timeToFade);
                track01.volume = Mathf.Lerp(maxVolume, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track01.Stop();
        }
        else
        {
            track01.clip = newClip;
            track01.Play();

            while (timeElapsed < timeToFade)
            {
                track01.volume = Mathf.Lerp(0, maxVolume, timeElapsed / timeToFade);
                track02.volume = Mathf.Lerp(maxVolume, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track02.Stop();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        track01 = bodyRb.gameObject.AddComponent<AudioSource>();
        track01.loop = true;
        track01.clip = idle;
        track01.volume = maxVolume;
        track01.spatialBlend = .8f;
        track01.Play();
        isPlayingTrack01 = is_idle = true;


        track02 = bodyRb.gameObject.AddComponent<AudioSource>();
        track02.volume = maxVolume;
        track02.spatialBlend = .8f;
    }

    // Update is called once per frame
    void Update()
    {
        frontWheelTransform.position = frontWheelRb.transform.position;
        frontWheelTransform.rotation = frontWheelRb.transform.rotation;

        backWheelTransform.position = backWheelRb.transform.position;
        backWheelTransform.rotation = backWheelRb.transform.rotation;

        bodyTransform.position = bodyRb.transform.position + (Vector3.up * -differentUpPosition);
        bodyTransform.rotation = bodyRb.transform.rotation;

        /*rig.position = bodyRb.transform.position + (Vector3.up * -differentRigUpPosition) +
            (Vector3.forward * -differentRigFwdPosition) +
            (Vector3.right * -differentRigRightPosition);
        rig.rotation = bodyRb.transform.rotation * Quaternion.Euler(Vector3.right * -90f);*/

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

        if (isPlayingTrack01)
        {
            if (!track01.isPlaying)
            {
                if (Mathf.Abs(velocity) < 15)
                    SwapTrack(idle);
                else
                    SwapTrack(topSpeed);
                if (isPlayingTrack01)
                    track01.loop = true;
                else
                    track02.loop = true;
            }
        }
        else
        {
            if (!track02.isPlaying)
            {
                if (Mathf.Abs(velocity) < 15)
                    SwapTrack(idle);
                else
                    SwapTrack(topSpeed);
                if (isPlayingTrack01)
                    track01.loop = true;
                else
                    track02.loop = true;
            }
        }


    }

    void Stabilizer()
    {
        Vector3 axisFromRotate = Vector3.Cross(bodyRb.transform.up, Vector3.up);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * torqueStabilizer;
        //torqueForce.x *= .4f;
        torqueForce.z *= .4f;
        torqueForce -= bodyRb.angularVelocity;
        bodyRb.AddTorque(torqueForce * bodyRb.mass * .02f, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (gameManager.IsInPlay())
        {
            //Vector3 accelleration = Vector3.zero;
            Vector3 torque = Vector3.zero;
            if (isGrounded)
            {
                bodyRb.AddForceAtPosition(bodyRb.transform.forward * accellerationForce * customVerticalAxis, 
                    bodyRb.transform.position + (bodyRb.transform.up * -applicationDeltaPoint));
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
        CalculateAccelleration();


    }
}
