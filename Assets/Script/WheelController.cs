using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform frontWheel;
    public Transform backWheel;
    public float rotationSpeed = 30f;

    private SuspensionBikeController suspensionBikeController;

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
    }

    // Update is called once per frame
    void Update()
    {
        float isReversed = Input.touchCount == 2 ? 1 : -1;

        //backWheel.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        //wheel.transform.Rotate(0, 0, rotationSpeed * isReversed * Time.deltaTime, Space.Self);
    }
}
