using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //la seconda condizione non viene valutata se la prima restituisce false
        if (gameManager.IsInPlay() && 
            other.CompareTag("Player") && 
            other.attachedRigidbody.velocity.magnitude <= .5f) 
        {
            if (name == "ParkingArea")
                gameManager.StartDelivery(gameObject.transform.parent.gameObject);
            else if (name == "ParkingSlot")
                gameManager.EndDelivery(gameObject.transform.parent.gameObject, true);
        }
    }
}
