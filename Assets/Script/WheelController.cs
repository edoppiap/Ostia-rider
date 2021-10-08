using UnityEngine;

public class WheelController : MonoBehaviour
{
    public GameObject[] wheelsToRotate;
    //public TrailRenderer[] trails;
    public TrailRenderer trail;
    public float rotationSpeed;
    public int sterzando;
    
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        sterzando = 0;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float isReversed = Input.touchCount == 2 ? 1 : -1;
        float sterzando = 0;
        if(Input.touchCount == 1)
            sterzando =  Input.GetTouch(0).position.x > Screen.width / 2 ? 1 : -1;

        foreach (var wheel in wheelsToRotate)
            wheel.transform.Rotate(0, 0, rotationSpeed * isReversed * Time.deltaTime, Space.Self);

        if(sterzando == 1)
        {
            anim.SetBool("sterzaDx", true);
            anim.SetBool("sterzaSx", false);
        }else if(sterzando == -1){
            anim.SetBool("sterzaDx", false);
            anim.SetBool("sterzaSx", true);
        }
        else
        {
            anim.SetBool("sterzaDx", false);
            anim.SetBool("sterzaSx", false);
        }

        if((sterzando != 0 || GetComponent<CarController>().isAccellerating()) && GetComponent<CarController>().isGrounded())
        {
            /*foreach(var trail in trails)
            {
                trail.emitting = true;
            }*/
            trail.emitting = true;
        }
        else
        {
            /*foreach(var trail in trails)
            {
                trail.emitting = false;
            }*/
            trail.emitting = false;
        }
    }
}
