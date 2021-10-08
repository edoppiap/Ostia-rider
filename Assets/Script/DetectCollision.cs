using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(Mathf.Abs(other.attachedRigidbody.velocity.z) <0.2f)
        {
            Debug.Log("Mi sono parcheggiato! Ho velocità: " + other.attachedRigidbody.velocity.z.ToString());
            if (name == "ParkingArea")
                FindObjectOfType<GameManager>().StartDelivery(gameObject.transform.parent.gameObject);
            else if (name == "ParkingSlot")
                FindObjectOfType<GameManager>().EndDelivery(gameObject.transform.parent.gameObject);
        }
    }
}
