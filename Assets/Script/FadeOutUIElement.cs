using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutUIElement : MonoBehaviour
{ 
    public float fadeTime = 2f;

    private Button button;
    private Color alphaZero;

    // Start is called before the first frame update
    void Start()
    {
        button = this.GetComponent<Button>();
        alphaZero = new Color(1, 1, 1, 0);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(4);
        while (button.image.color.a > 0.001f)
        {
            button.image.color = Color.Lerp(button.image.color, alphaZero, fadeTime * Time.deltaTime);
            yield return null;
        }
        Destroy(transform.parent.gameObject);
    }
}
