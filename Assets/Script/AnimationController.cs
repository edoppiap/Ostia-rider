using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animators")]
    public Animator characterAnimator;
    public Animator coperchioAnimator;
    private Animator bikeAnimator;

    [Header("Motorbike parts")]
    public Transform frontWheelMesh;
    public Transform frontWheelTransform;
    public SphereCollider frontWheelCollider;
    public Rigidbody backWheelRb;
    public Transform backWheelMesh;
    public Transform manubrio;
    public Transform parafanghi;

    [Header("Parameters")]
    public float rotateAngle = 20f;
    public float rightAngle = 5f;

    private SuspensionBikeController suspensionBikeController;
    private Rigidbody frontWheelRb;
    private bool frontWheelGrounded;

    Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    // Start is called before the first frame update
    void Start()
    {
        bikeAnimator = GetComponent<Animator>();
        suspensionBikeController = GetComponent<SuspensionBikeController>();
        frontWheelRb = frontWheelCollider.attachedRigidbody;
    }

    // Update is called once per frame
    void Update()
    {
        //public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance);
        //raycast ground check
        RaycastHit hit;
        frontWheelGrounded = Physics.Raycast(frontWheelCollider.transform.position, -frontWheelCollider.transform.up, out hit, frontWheelCollider.radius);
        if (frontWheelGrounded)
        {
            Debug.DrawRay(frontWheelCollider.transform.position, -frontWheelCollider.transform.transform.up * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(frontWheelCollider.transform.position, -frontWheelCollider.transform.transform.up * frontWheelCollider.radius, Color.white);
        }
        float fwrVelocity = frontWheelGrounded ? Vector3.Dot(frontWheelRb.transform.forward, frontWheelRb.velocity) * 100 : 0f;

        //wheels rotation
        backWheelMesh.Rotate(Vector3.right, suspensionBikeController.GetAccellerationForce() * Time.deltaTime);
        frontWheelMesh.Rotate(Vector3.right, fwrVelocity * Time.deltaTime);

        //animation
        if(suspensionBikeController.GetCustomHorizontal() > 0)
        {
            characterAnimator.SetBool("goingLeft", false);
            characterAnimator.SetBool("goingRight", true);

            bikeAnimator.SetBool("goingLeft", false);
            bikeAnimator.SetBool("goingRight", true);
        }
        else if(suspensionBikeController.GetCustomHorizontal() < 0)
        {
            characterAnimator.SetBool("goingRight", false);
            characterAnimator.SetBool("goingLeft", true);

            bikeAnimator.SetBool("goingRight", false);
            bikeAnimator.SetBool("goingLeft", true);
        }
        else
        {
            characterAnimator.SetBool("goingLeft", false);
            characterAnimator.SetBool("goingRight", false);

            bikeAnimator.SetBool("goingRight", false);
            bikeAnimator.SetBool("goingLeft", false);
        }

        coperchioAnimator.SetBool("isNotGrounded", backWheelRb.velocity.y < -5);
    }
}
