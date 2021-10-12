using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera menuCamera;
    public CinemachineVirtualCamera optionsCamera;
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    //public Canvas MainMenuCanvas;

    [Header("Components to hide")]
    public GameObject settings3DIcon;
    public GameObject logoIcon;
    public GameObject playCollider;

    [Header("Parents")]
    public GameObject restaurantsParent;
    public GameObject clientsParent;
    public GameObject arrow;

    private List<GameObject> restaurants = new List<GameObject>();
    private List<GameObject> clients = new List<GameObject>();
    private GameObject tempRestaurant;

    public void Pause()
    {
        pauseCanvas.SetActive(true);
        gameCanvas.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        Debug.Log("Hai cliccato anche opzioni");
        optionsCamera.Priority = 1;
        menuCamera.Priority = 0;
    }

    public void CloseOptions()
    {
        optionsCamera.Priority = 0;
        menuCamera.Priority = 1;
    }

    private void DeAssignClient()
    {
        tempRestaurant.GetComponent<ClienteAssegnato>().SetCliente(null);
    }

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

            restaurant.GetComponent<ClienteAssegnato>().SetActiveBasedOnProbability();
        }
    }

    public void Clicked()
    {
        Debug.Log("Mi hai cliccato");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        StartGame();
    }

    public void StartGame()
    {
        mainCamera.Priority = 1;
        menuCamera.Priority = 0;

        gameCanvas.SetActive(true);

        settings3DIcon.SetActive(false);
        logoIcon.SetActive(false);
        playCollider.SetActive(false);
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
        tempRestaurant = restaurant;

        AssignTarget(restaurant.GetComponent<ClienteAssegnato>().GetCliente().transform);
    }

    public void EndDelivery(GameObject client)
    {
        //clients.Add(client);
        client.SetActive(false);

        DeAssignClient();
        AssignClient();
        ReactivateAll(restaurants);

        DeAssignTarget();
    }

    // Start is called before the first frame update
    public void Start()
    {
        menuCamera.Priority = 1;
        mainCamera.Priority = 0;

        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(false);

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
