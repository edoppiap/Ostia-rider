using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    CarSpawner carSpawner;
    void OnBecameInvisible()
    {
        if (transform.parent.GetComponent<TrafficCarController>().touched)
            carSpawner.DisableCar(transform.parent.gameObject);
                //Destroy(transform.parent.gameObject);

    }

    public void SetCarSpawner(CarSpawner carSpawner)
    {
        this.carSpawner = carSpawner;
    }
}
