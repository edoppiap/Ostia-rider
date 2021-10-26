using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    public Material[] materials;

    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnBecameInvisible()
    {
        if (transform.parent.GetComponent<TrafficCarController>().hit)
            Destroy(transform.parent.gameObject);
    }

    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials[0] = materials[Random.Range(0, materials.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
