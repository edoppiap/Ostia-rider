using UnityEngine;

public class WheelController : MonoBehaviour
{
    [Header("Motorbike parts")]
    public Transform frontWheelMesh;
    public Transform frontWheelTransform;
    public SphereCollider frontWheelCollider;
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

    /*public void SterzaDx()
    {
        sterzando = 1;
        if (accellerando)
        {
            sterzando = true;
            anim.SetBool("sterzaDx", true);
        }
    }

    public void Accellera()
    {
        accellerando = true;
    }

    public void Decellera()
    {
        accellerando = false;
    }

    public void DeSterza()
    {
        sterzando = false;
        anim.SetBool("sterzaDx", false);
        anim.SetBool("sterzaSx", false);
    }

    public void SterzaSx()
    {
        if (accellerando)
        {
            anim.SetBool("sterzaSx", true);
            sterzando = true;
        }
        //sterzando = -1;
    }*/

    // Start is called before the first frame update
    void Start()
    {
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

        //wheel direction
        frontWheelTransform.localEulerAngles = Vector3.up * suspensionBikeController.GetCustomHorizontal() * rotateAngle;
        manubrio.localEulerAngles = new Vector3(-suspensionBikeController.GetCustomHorizontal() * 5f, suspensionBikeController.GetCustomHorizontal() * rotateAngle/2, 0f);
        //parafanghi.localEulerAngles =  Vector3.up* suspensionBikeController.GetCustomHorizontal() * rotateAngle;
        //frontWheel.Rotate(frontWheel.up, suspensionBikeController.GetCustomHorizontal()*20f);
    }
}
