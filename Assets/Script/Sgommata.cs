using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sgommata : MonoBehaviour
{
    public TrailRenderer trailPrefab;
    public ParticleSystem smokePrefab;
    public float startEmitting = 10f;
    public bool isBack = false;
    public float emittionAccelleration = 10f;
    public float stopEmittingVelocity = 20f;

    private SuspensionBikeController bikeController;
    private Vector3 lateralVelocity = Vector3.zero;
    private Rigidbody rb;
    private AudioSource audioSource;

    bool BoolEmittingBasedOnAccelleration()
    {
        float velocity = Vector3.Dot(rb.velocity, rb.transform.forward);
        return isBack &&
            Mathf.Abs(bikeController.GetAccelleration()) > emittionAccelleration &&
            velocity >= 0 &&
            velocity < stopEmittingVelocity;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") && 
            (lateralVelocity.magnitude >= startEmitting || (BoolEmittingBasedOnAccelleration())))
        {
            trailPrefab.emitting = true;
            smokePrefab.Play();
            audioSource.mute = false;
        }
        else
        {
            trailPrefab.emitting = false;
            smokePrefab.Stop();
            audioSource.mute = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        trailPrefab.emitting = false;
        smokePrefab.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        bikeController = transform.parent.GetComponent<SuspensionBikeController>();
        audioSource = transform.GetComponent<AudioSource>();
        trailPrefab.emitting = false;
        smokePrefab.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        lateralVelocity = Vector3.Dot(rb.transform.right, rb.velocity) * rb.transform.right;
    }
}
