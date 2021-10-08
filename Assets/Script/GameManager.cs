using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject restaurantsParent;
    public GameObject clientsParent;

    private List<GameObject> restaurants = new List<GameObject>();
    private List<GameObject> clients = new List<GameObject>();

    private void AssignClient()
    {
        foreach (GameObject restaurant in restaurants)
        {
            ClienteAssegnato scriptCliente = restaurant.GetComponent<ClienteAssegnato>();

            if (clients.Count != 0 && scriptCliente.GetCliente() == null)
            {
                Random.InitState(System.DateTime.Now.Millisecond);
                int i = Mathf.RoundToInt(Random.Range(0, clients.Count - 1));
                scriptCliente.SetCliente(clients[i]);
                clients.RemoveAt(i);
            }

            if (scriptCliente.GetCliente() == null)
            {
                restaurant.SetActive(false);
            }
        }
    }

    private void PopulateList(Transform[] array, List<GameObject> listToPopulate)
    {
        foreach(var temp in array)
        {
            if (temp.gameObject.layer == 12)
                listToPopulate.Add(temp.gameObject);
        }
    }

    public void DeactivateAll(List<GameObject> list)
    {
        foreach (var temp in list)
            temp.SetActive(false);
    }

    public void ReactivateAll(List<GameObject> list)
    {
        foreach (var temp in list)
        {
            if(temp.name.Contains("Ristorante") && temp.GetComponent<ClienteAssegnato>().GetCliente() != null)
                temp.SetActive(true);
        }
    }

    public void StartDelivery(GameObject restaurant)
    {
        Debug.Log("Inizio consegna da " + restaurant.ToString());
        DeactivateAll(restaurants);
        restaurant.GetComponent<ClienteAssegnato>().GetCliente().SetActive(true);
    }

    public void EndDelivery(GameObject client)
    {
        Debug.Log("Consegna Effettuata a "+ client.ToString());
        clients.Add(client);
        client.SetActive(false);
        AssignClient();
        ReactivateAll(restaurants);
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateList(restaurantsParent.GetComponentsInChildren<Transform>(), restaurants);
        PopulateList(clientsParent.GetComponentsInChildren<Transform>(), clients);

        DeactivateAll(clients);
        AssignClient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
