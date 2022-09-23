using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticles : MonoBehaviour
{
    public ParticleSystem particle;
    public string[] destroyTag = { "Player", "Car" };

    bool CompareTags(string tag)
    {
        foreach (string t in destroyTag)
        {
            if (t.Equals(tag))
                return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTags(collision.gameObject.tag))
        {
            Transform[] childs = GetComponentsInChildren<Transform>();
            foreach (Transform child in childs)
            {
                if (!child.Equals(this.transform))
                    Instantiate(particle, child.position, child.rotation, child.parent);
            }
        }
    }
}
