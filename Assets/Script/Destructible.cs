using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;
    public ParticleSystem distructEffect;
    public bool ParticlesAtPosition;
    public bool ParticlesAtContactPoint;
    public string[] destroyTag = {"Player", "Car"};

    private bool isAlreadySpawned = false;
    
    bool CompareTags(string tag)
    {
        foreach(string t in destroyTag)
        {
            if (t.Equals(tag))
                return true;
        }
        return false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTags(collision.gameObject.tag) && !isAlreadySpawned)
        {
            isAlreadySpawned = true;
            if(destroyedVersion != null)
            {
                GameObject instantiated = Instantiate(destroyedVersion, transform.position, transform.rotation, transform.parent);
                if(instantiated.TryGetComponent(out Renderer renderer))
                    renderer.material = gameObject.GetComponent<Renderer>().material;
            }
            Destroy(gameObject);

            if (distructEffect != null)
            {
                if(ParticlesAtPosition)
                    Instantiate(distructEffect, transform.position, transform.rotation, transform.parent);
                else if(ParticlesAtContactPoint)
                    Instantiate(distructEffect, collision.GetContact(0).point, transform.rotation, transform.parent);
            }

        }
    }
    private void OnDestroy()
    {
        
    }
}
