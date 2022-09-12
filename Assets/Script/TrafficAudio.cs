using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficAudio : MonoBehaviour
{
    public AudioClip crashClip, clacsonClip;
    public string[] destroyTag = { "Player", "Car" };
    AudioSource crashSource, clacsonSource;
    GameObject player;

    bool CompareTags(string tag)
    {
        foreach (string t in destroyTag)
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

        player = GameObject.Find("Collider");
    }

    // Update is called once per frame
    void Update()
    {
        if(!clacsonSource.isPlaying && (Vector3.Distance(gameObject.transform.position, player.transform.position) < 8))
        {
            clacsonSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null)
        {
            if (!crashSource.isPlaying && CompareTags(collision.gameObject.tag))
            {
                crashSource.Play();
            }
        }
    }
}
