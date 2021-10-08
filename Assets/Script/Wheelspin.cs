using UnityEngine;

public class Wheelspin : MonoBehaviour
{
    public GameObject[] wheelsToRotate;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float isReversed = Input.touchCount == 2 ? 1 : -1;

        foreach (var wheel in wheelsToRotate)
            wheel.transform.Rotate(0, 0, rotationSpeed * isReversed * Time.deltaTime, Space.Self);
    }
}
