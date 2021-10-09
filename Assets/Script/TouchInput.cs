using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    /*
    public float Acceleration
    {
        get { return m_Acceleration; }
    }
    public float Steering
    {
        get { return m_Steering; }
    }
    public bool BoostPressed
    {
        get { return m_BoostPressed; }
    }
    public bool FirePressed
    {
        get { return m_FirePressed; }
    }
    public bool HopPressed
    {
        get { return m_HopPressed; }
    }
    public bool HopHeld
    {
        get { return m_HopHeld; }
    }

    float m_Acceleration;
    float m_Steering;
    bool m_HopPressed;
    bool m_HopHeld;
    bool m_BoostPressed;
    bool m_FirePressed;

    Vector2 firstPosition;

    bool m_forward;
    bool m_FirstSteeringPressed;

    bool m_FixedUpdateHappened;

    private void Start()
    {
        m_forward = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_forward)
        {
            m_Acceleration = 1f;
        }
        else
        {
            m_Acceleration = -1f;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                m_FirstSteeringPressed = true;
                firstPosition = touch.position;
            }

        m_Steering = Vector2.Distance(firstPosition, touch.position);
        }
    }

    private void FixedUpdate()
    {
        m_FixedUpdateHappened = true;
    }*/
}
