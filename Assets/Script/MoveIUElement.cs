using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIUElement : MonoBehaviour
{
    public float moveTime = 5f;
    Vector3 destination = Vector3.zero;
    bool temp = false;

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    private void Awake()
    {
    }

    private void Update()
    {
        if(destination != Vector3.zero && !temp)
        {
            temp = true;
            LeanTween.move(gameObject, destination, moveTime);
        }
    }
}
