using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FadeOutUIElement : MonoBehaviour
{ 
    public float fadeTime = .05f;

    private Color alphaZero;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        TextMeshProUGUI[] childsText = new TextMeshProUGUI[transform.childCount];
        //yield return new WaitForSeconds(4);
        int i = 0;
        foreach (Transform child in transform)
        {
            childsText[i] = child.GetComponent<TextMeshProUGUI>();
            i++;
        }

        alphaZero = new Color(0,0,0,0);
        float time = 0;
        while (childsText[0].faceColor.a > 6f)
        {
            time += Time.deltaTime;
            foreach(TextMeshProUGUI text in childsText)
            {
                text.faceColor = Color.Lerp(text.faceColor, alphaZero, fadeTime * time);

            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
