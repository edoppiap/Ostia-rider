using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera mainMenuCamera;
    public Canvas GameCanvas;
    public Canvas MainMenuCanvas;

    [Header("Components to hide")]
    public GameObject settings3DIcon;
    public GameObject logoIcon;

    [Header("Parents")]
    public GameObject restaurantsParent;
    public GameObject clientsParent;
    public GameObject arrow;

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
                int i = Mathf.RoundToInt(Random.Range(0, clients.Count));
                scriptCliente.SetCliente(clients[i]);
                //clients.RemoveAt(i);
            }

            if (scriptCliente.GetCliente() == null)
            {
                restaurant.SetActive(false);
            }
        }
    }

    public void Clicked()
    {
        Debug.Log("Mi hai cliccato");
    }

    public void StartGame()
    {
        mainCamera.Priority = 1;
        mainMenuCamera.Priority = 0;

        MainMenuCanvas.enabled = false;
        GameCanvas.enabled = true;

        settings3DIcon.active = false;
        logoIcon.active = false;
    }

    private void AssignTarget(Transform target)
    {
        arrow.SetActive(true);
        arrow.GetComponent<PointAt>().SetTarget(target);
    }

    private void DeAssignTarget()
    {
        arrow.SetActive(false);
        arrow.GetComponent<PointAt>().SetTarget(null);

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
        DeactivateAll(restaurants);
        restaurant.GetComponent<ClienteAssegnato>().GetCliente().SetActive(true);

        AssignTarget(restaurant.GetComponent<ClienteAssegnato>().GetCliente().transform);
    }

    public void EndDelivery(GameObject client)
    {
        clients.Add(client);
        client.SetActive(false);
        AssignClient();
        ReactivateAll(restaurants);

        DeAssignTarget();
    }

    // Start is called before the first frame update
    public void Start()
    {
        mainMenuCamera.Priority = 1;
        mainCamera.Priority = 0;

        MainMenuCanvas.enabled = true;
        GameCanvas.enabled = false;

        PopulateList(restaurantsParent.GetComponentsInChildren<Transform>(), restaurants);
        PopulateList(clientsParent.GetComponentsInChildren<Transform>(), clients);

        DeactivateAll(clients);
        AssignClient();

        arrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
