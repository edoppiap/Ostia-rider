using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera menuCamera;
    public CinemachineVirtualCamera optionsCamera;

    [Header("Canvas")]
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public GameObject gameOverCanvas;
    //public Canvas MainMenuCanvas;

    [Header("Timer canvas")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI totalText;
    public float gameTime = 61f;
    private float timeRemaining;

    [Header("Components to hide")]
    public GameObject[] objectsToHide;

    [Header("Parents")]
    public GameObject restaurantsParent;
    public GameObject clientsParent;
    public GameObject arrow;

    private List<GameObject> restaurants = new List<GameObject>();
    private List<GameObject> clients = new List<GameObject>();
    private GameObject tempRestaurant;
    private static bool reloaded = false;
    private static bool inPlay = false;
    private bool gameHasEnded = false;
    private int countDelivery = 0;

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public void AddTime(float addingTime)
    {
        timeRemaining += addingTime;
    }

    public bool IsInPlay()
    {
        return inPlay;
    }

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

    public void EndGame()
    {
        gameHasEnded = true;
        gameOverCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        totalText.SetText("Total delivery: " + countDelivery.ToString());
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

    public void Restart()
    {
        reloaded = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void BackToMenu()
    {
        inPlay = false;
        reloaded = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void StartGame()
    {
        inPlay = true;
        gameHasEnded = false;
        mainCamera.Priority = 1;
        menuCamera.Priority = 0;

        gameCanvas.SetActive(true);

        foreach(var icon in objectsToHide){
            icon.SetActive(false);
        }
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
        countDelivery++;

        DeAssignClient();
        AssignClient();
        ReactivateAll(restaurants);

        DeAssignTarget();
    }

    // Start is called before the first frame update
    public void Start()
    {
        timeRemaining = gameTime;
        PopulateList(restaurantsParent.GetComponentsInChildren<Transform>(), restaurants);
        PopulateList(clientsParent.GetComponentsInChildren<Transform>(), clients);

        DeactivateAll(clients);
        AssignClient();

        arrow.SetActive(false);

        if (!reloaded)
        {
            menuCamera.Priority = 1;
            mainCamera.Priority = 0;

            pauseCanvas.SetActive(false);
            gameCanvas.SetActive(false);
            gameOverCanvas.SetActive(false);
        }
        else
        {
            gameOverCanvas.SetActive(false);
            pauseCanvas.SetActive(false);
            StartGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timeRemaining > 0 && inPlay)
        {
            timeRemaining -= Time.deltaTime;
            if(timeRemaining > 0)
                timerText.SetText(Mathf.FloorToInt(timeRemaining).ToString());
            else
                timerText.SetText("0");
        }else if(timeRemaining <= 0 && !gameHasEnded)
        {
            EndGame();
        }
    }
}
