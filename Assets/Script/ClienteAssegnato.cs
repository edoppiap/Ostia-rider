using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClienteAssegnato : MonoBehaviour
{
    private GameObject cliente;
    private Renderer[] renderers;
    private Collider colliderCube;

    [Header("Distanze")]
    public float minDistance = 300f;
    public float maxDistance = 500f;
    
    private void Start()
    {
        colliderCube = transform.GetComponent<Collider>();
        renderers = transform.GetComponentsInChildren<Renderer>();
        //materials = transform.GetComponentInChildrens<Renderer>().material;
    }

    public GameObject GetCliente()
    {
        return this.cliente;
    }

    public void SetCliente(GameObject cliente)
    {
        this.cliente = cliente;
        CalculateColor();
    }

    /*public void StartDelivery()
    {
        //colliderCube;
    }*/

    public void CalculateColor()
    {
        float dist = GetDistanceFromClient();
        foreach(var renderer in renderers)
        {
            Material material = renderer.material;

            if(dist < minDistance)
                material.SetColor("_Color", Color.red);
            else if(dist >= minDistance && dist <= maxDistance)
                material.SetColor("_Color", Color.yellow);
            else if(dist > maxDistance)
                material.SetColor("_Color", Color.green);

            if (renderer.gameObject.name == "ParkingArea") { 
                material.SetColor("_Color", new Color(material.color.r, material.color.g, material.color.b, .5f));
            }
        }
        
    }

    public float GetDistanceFromClient()
    {
        if(cliente != null)
            return Vector3.Distance(transform.position, cliente.transform.position);
        return 0;
    }

    private void Update()
    {
        
    }

}
