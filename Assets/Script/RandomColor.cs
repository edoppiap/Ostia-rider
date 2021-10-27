using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    public Material[] materials;

    private Renderer rendererComponent;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake()
    {
        rendererComponent = GetComponent<Renderer>();
        Material tempMaterial = materials[Random.Range(0, materials.Length)];
        rendererComponent.material = tempMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
