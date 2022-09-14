using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtCollision : MonoBehaviour
{
    public AudioClip clip;
    public string[] tags = { "Player", "Car" };
    AudioSource source;

    bool CompareTags(string tag)
    {
        foreach (string t in tags)
        {
            if (t.Equals(tag))
                return true;
        }
        return false;
    }

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = .8f;
        source.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!source.isPlaying && CompareTag(collision.gameObject.tag))
        {
            source.Play();
        }
    }
}
