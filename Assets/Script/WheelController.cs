using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform frontWheel;
    public SphereCollider frontWheelCollider;
    public Transform backWheel;

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

        backWheel.Rotate(Vector3.right, suspensionBikeController.GetAccellerationForce() * Time.deltaTime);
        frontWheel.Rotate(Vector3.right, fwrVelocity * Time.deltaTime);
    }
}
