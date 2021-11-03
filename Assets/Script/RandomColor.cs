using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    public Material[] materials;

    private Renderer rendererComponent;

    private void OnEnable()
    {
        rendererComponent = GetComponent<Renderer>();
        Material tempMaterial = materials[Random.Range(0, materials.Length)];
        rendererComponent.material = tempMaterial;
    }
}
