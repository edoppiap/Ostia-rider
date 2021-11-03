using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    private Transform target;
    private Vector3 oldPosition;

    private void OnDisable()
    {
        //LeanTween.scale(gameObject, Vector3.zero, .5f);
    }

    private void OnEnable()
    {
        //LeanTween.scale(gameObject, Vector3.one, .5f);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Start()
    {
        if (transform.parent.CompareTag("Places"))
            target = GameObject.Find("Corpo_Centrale").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && target != null)
        {
            Vector3 direction = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
