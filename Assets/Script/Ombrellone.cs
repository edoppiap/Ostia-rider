using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ombrellone : MonoBehaviour
{
    [Range(0,1)]
    public float percentualeSpawn = .7f;

    public GameObject ombrelloneAperto;
    public GameObject lettino;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0f, 1f) < percentualeSpawn)
        {
            Vector3 position = gameObject.transform.Find("SpawnLettino").position;
            Material material = gameObject.GetComponent<Renderer>().material;

            GameObject ombrellone = Instantiate(ombrelloneAperto, transform.position, transform.rotation, transform.parent);
            ombrellone.GetComponent<Renderer>().material = material;

            GameObject lettinoInstantiated = Instantiate(lettino, position, transform.rotation, transform.parent);
            lettinoInstantiated.GetComponent<Renderer>().material = material;

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
