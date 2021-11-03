using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PhysicMaterial zeroFrictionMaterial;

    [Header("Cameras")]
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera menuCamera;
    public CinemachineVirtualCamera optionsCamera;

    [Header("Canvas")]
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public GameObject gameOverCanvas;
    public GameObject deliveryTimeCanvas;

    [Header("Text component")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI addedTimeText;
    public TextMeshProUGUI deliveryTimeText;
    public TextMeshProUGUI messageText;

    [Header("Time parameters")]
    public float timeAddedFor100Metres = 6f;
    public float gameTime = 61f;
    public int timeForDeliveryFor100Metres = 20;
    private float timeRemaining;

    [Header("Components to hide")]
    public GameObject[] objectsToHide;

    [Header("Parents")]
    public GameObject restaurantsParent;
    public GameObject clientsParent;
    public GameObject arrow;

    [Header("Speed delivery variables")]
    [Range(0f, 1f)]
    public float minSpeedyBonus = .7f;
    [Range(0f, 1f)]
    public float minNormalBonus = .3f;

    private List<GameObject> restaurants = new List<GameObject>();
    private List<GameObject> clients = new List<GameObject>();
    private GameObject tempRestaurant;
    private static bool reloaded = false;
    private static bool inPlay = false;
    private bool gameHasEnded = false;
    private int countDelivery = 0;
    private float deliveryTimeRemaining = 100f;
    private float deliveryTime;
    private float startDeliveryTime;

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    IEnumerator FadeOut(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void AddTime(float addingTime)
    {
        addedTimeText.gameObject.SetActive(true);
        addedTimeText.SetText("+" + Mathf.FloorToInt(addingTime).ToString());
        timeRemaining += addingTime;
        StartCoroutine(FadeOut(addedTimeText.gameObject, 4f));
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
        inPlay = false;
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
        addedTimeText.gameObject.SetActive(false);
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
        ClienteAssegnato clienteAssegnato = restaurant.GetComponent<ClienteAssegnato>();
        DeactivateAll(restaurants);
        clienteAssegnato.GetCliente().SetActive(true);
        tempRestaurant = restaurant;
        AddTime(timeAddedFor100Metres*(clienteAssegnato.GetDistanceFromClient())/100);
        CalculateDeliveryTime(clienteAssegnato.GetDistanceFromClient());
        AssignTarget(clienteAssegnato.GetCliente().transform);
        startDeliveryTime = Time.time;
        deliveryTimeCanvas.gameObject.SetActive(true);
    }

    void CalculateDeliveryTime(float distance)
    {
        deliveryTimeRemaining = timeForDeliveryFor100Metres * (distance / 100);
        deliveryTimeRemaining = timeRemaining >= deliveryTimeRemaining ? deliveryTimeRemaining : timeRemaining;
        deliveryTime = deliveryTimeRemaining;
    }

    void SetColorAndTextAndThenFadeOut(TextMeshProUGUI text, Color color, string stringa)
    {
        text.gameObject.SetActive(true);
        text.SetText(stringa);
        text.faceColor = color;
        StartCoroutine(FadeOut(text.gameObject, 2f));
    }

    public void EndDelivery(GameObject client, bool done)
    {
        deliveryTimeCanvas.gameObject.SetActive(false);
        if (deliveryTimeRemaining > deliveryTime * minSpeedyBonus)
        {
            AddTime(5f);
            SetColorAndTextAndThenFadeOut(messageText, Color.green, "SPEEDY!");
        }
        else if (deliveryTimeRemaining > deliveryTime * minNormalBonus)
        {
            AddTime(2f);
            SetColorAndTextAndThenFadeOut(messageText, Color.yellow, "NORMAL");
        }
        else if (done)
        {
            SetColorAndTextAndThenFadeOut(messageText, Color.red, "SLOW!");
        }else
        {
            SetColorAndTextAndThenFadeOut(messageText, Color.red, "MISSED!");
        }

        client.SetActive(false);
        if(done)
            countDelivery++;

        DeAssignClient();
        AssignClient();
        ReactivateAll(restaurants);

        DeAssignTarget();
    }

    void EndDelivery()
    {
        EndDelivery(tempRestaurant.GetComponent<ClienteAssegnato>().GetCliente(), false);
    }

    void SetTextIfNotMinusZero(TextMeshProUGUI text, float time)
    {
        if (time > 0)
            text.SetText(Mathf.FloorToInt(time).ToString());
        else
            text.SetText("0");
    }

    void SetTextColorBasedOnSpeedDelivery()
    {
        if (deliveryTimeRemaining > deliveryTime * minSpeedyBonus)
            deliveryTimeText.faceColor = Color.green;
        else if (deliveryTimeRemaining > deliveryTime * minNormalBonus)
            deliveryTimeText.faceColor = Color.yellow;
        else
            deliveryTimeText.faceColor = Color.red;
    }

    void CheckTime()
    {
        if (timeRemaining > 0 && inPlay)
        {
            timeRemaining -= Time.deltaTime;
            SetTextIfNotMinusZero(timerText, timeRemaining);

            if (deliveryTimeText.IsActive() && deliveryTimeRemaining > 0)
            {
                deliveryTimeRemaining -= Time.deltaTime;
                SetTextIfNotMinusZero(deliveryTimeText, deliveryTimeRemaining);

                SetTextColorBasedOnSpeedDelivery();
            }
            else if (deliveryTimeRemaining <= 0 && deliveryTimeText.IsActive())
                EndDelivery();
        }
        else if (timeRemaining <= 0 && !gameHasEnded)
        {
            EndGame();
        }
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
        deliveryTimeCanvas.transform.SetParent(GameObject.Find("Borsone").transform);
        deliveryTimeCanvas.SetActive(false);
        messageText.gameObject.SetActive(false);

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
        CheckTime();
        if (!inPlay)
            zeroFrictionMaterial.dynamicFriction = 1f;
        else
            zeroFrictionMaterial.dynamicFriction = 0f;
    }
}
