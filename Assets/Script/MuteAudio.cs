using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteAudio : MonoBehaviour
{
    public GameObject volumeUp, volumeDown;
    
    public void StopListening()
    {
        AudioListener.volume = 0;
        volumeDown.SetActive(true);
        volumeUp.SetActive(false);
    }

    public void StartListening()
    {
        AudioListener.volume = 1;
        volumeDown.SetActive(false);
        volumeUp.SetActive(true);
    }
}
