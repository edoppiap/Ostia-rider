using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivator : MonoBehaviour
{
    private GameObject mainCamera;
    [HideInInspector]
    public List<ActivatorItem> activatorItems;

    public int distanceFromPlayer;
    public GameObject disableParent;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        activatorItems = new List<ActivatorItem>();

        foreach(Transform child in disableParent.transform)
        {
            activatorItems.Add(new ActivatorItem { item = child.gameObject, itemPos = child.position });
        }

        StartCoroutine(CheckActivation());
    }

    IEnumerator CheckActivation()
    {
        List<ActivatorItem> removeList = new List<ActivatorItem>();

        foreach (ActivatorItem item in activatorItems)
        {
            if(Vector3.Distance(mainCamera.transform.position, item.itemPos) > distanceFromPlayer)
            {
                if(item.item == null)
                {
                    removeList.Add(item);
                }
                else
                {
                    item.item.SetActive(false);
                }
            }
            else
            {
                if(item.item == null)
                {
                    removeList.Add(item);
                }
                else
                {
                    item.item.SetActive(true);
                }
            }
        }

        yield return new WaitForSeconds(.01f);

        if (removeList.Count > 0) 
        {
            foreach (ActivatorItem item in removeList)
            {
                activatorItems.Remove(item);
            }
        }

        yield return new WaitForSeconds(.01f);
        StartCoroutine(CheckActivation());
    }
}

public class ActivatorItem
{
    public GameObject item;
    public Vector3 itemPos;
}
