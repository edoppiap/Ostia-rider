using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRadius : MonoBehaviour
{
    private Material material;
    //private SphereCollider sphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        //sphereCollider = GetComponent<SphereCollider>();
        //radius = sphereCollider.radius;
        //Debug.Log(this.name);
    }

    // Update is called once per frame
    void Update()
    {
        if(material.color == Color.green){
            transform.localScale = new Vector3(.5f, 1f, .5f);
            //sphereCollider.radius = 15f * 0.5f;
        }else if(material.color == Color.red)
        {
            transform.localScale = new Vector3(1.5f, 1f, 1.5f);
        }
    }
}
