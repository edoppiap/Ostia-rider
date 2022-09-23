using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIfFarAway : MonoBehaviour
{
    private ItemActivator activationScript;

    // Start is called before the first frame update
    void Start()
    {
        activationScript = GameObject.Find("GameManager").GetComponent<ItemActivator>();

        StartCoroutine(AddToList());
    }

    IEnumerator AddToList()
    {
        yield return new WaitForSeconds(.01f);
        activationScript.activatorItems.Add(new ActivatorItem { item = this.gameObject, itemPos = transform.position });
    }
}
