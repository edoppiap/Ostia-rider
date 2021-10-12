using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBlink : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.value(gameObject, text.color.a, 0f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
