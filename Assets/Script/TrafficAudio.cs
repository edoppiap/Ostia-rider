using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficAudio : MonoBehaviour
{
    public AudioClip crashClip, clacsonClip;
    public string[] playTag= { "Player"};
    AudioSource crashSource, clacsonSource;

    bool CompareTags(string tag)
    {
        foreach (string t in playTag)
        {
            if (t.Equals(tag))
                return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        crashSource = gameObject.AddComponent<AudioSource>();
        crashSource.clip = crashClip;
        crashSource.spatialBlend = .8f;
        crashSource.playOnAwake = false;

        clacsonSource = gameObject.AddComponent<AudioSource>();
        clacsonSource.clip = clacsonClip;
        clacsonSource.spatialBlend = 1f;
        clacsonSource.playOnAwake = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (clacsonSource!=null && !clacsonSource.isPlaying && CompareTags(other.tag))
        {
            clacsonSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(crashSource != null)
        {
            if (!crashSource.isPlaying && CompareTags(collision.gameObject.tag))
            {
                crashSource.Play();
            }
        }
    }
}
