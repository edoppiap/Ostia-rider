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
    public GameObject startCanvas;
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public GameObject optionsCanvas;
    public GameObject gameOverCanvas;
    public GameObject deliveryTimeCanvas;

    [Header("Text component")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI totalText;
    public GameObject bonusPrefab;
    public TextMeshProUGUI deliveryTimeText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI globalMoneyText;
    public TextMeshProUGUI localMoneyText;
    public TextMeshProUGUI recordText;
    public TextMeshProUGUI tapToStartText;

    [Header("Time parameters")]
    public float timeAddedFor100Metres = 6f;
    public float gameTime = 61f;
    public int timeForDeliveryFor100Metres = 20;
    private float timeRemaining;
    public int feeForDeliveryFor100Metres = 2;

    [Header("Bonus parameters")]
    public string speedyText = "SPEEDY!";
    public string normalText = "NORMAL";
    public string badText = "BAD!";
    public string missedText = "MISSED!";
    public float speedyTimeBonus = 5f;
    public float normalTimeBonus = 2f;
    public int speedyTip = 5;
    public int normalTip = 3;
    public string bonusTimeString = "Bonus time:";
    public string feeMoneyString = "Fee:";
    public string tipMoneyString = "Tip:";

    [Header("Parents")]
    public GameObject restaurantsParent;
    public GameObject clientsParent;
    public GameObject arrow;

    [Header("Speed delivery variables")]
    [Range(0f, 1f)]
    public float minSpeedyBonus = .7f;
    [Range(0f, 1f)]
    public float minNormalBonus = .3f;

    private Player player;
    private List<GameObject> restaurants = new List<GameObject>();
    private List<GameObject> clients = new List<GameObject>();
    private GameObject tempRestaurant;
    private static bool reloaded = false;
    private static bool inPlay = false;
    private bool gameHasEnded = false;
    private int countDelivery = 0;
    private float deliveryTimeRemaining = 100f;
    private float deliveryTime;
    private int localMoney = 0;

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    IEnumerator FadeOut(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(player);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if(data != null)
        {
            player.globalMoney = data.globalMoney;
            player.record = data.record;
        }
        else
        {
            player.globalMoney = 0;
            player.record = 0;
        }
    }

    IEnumerator DoubleBonusPrefab(int fee, int tip)
    {
        GameObject feePrefab = Instantiate(bonusPrefab, localMoneyText.transform.position - Vector3.up * 150f, timerText.transform.parent.rotation, gameCanvas.transform);
        feePrefab.GetComponent<MoveIUElement>().SetDestination(localMoneyText.transform.position);
        feePrefab.GetComponent<AssignBonusText>().SetBonusText(feeMoneyString + " ", Mathf.FloorToInt(fee).ToString());

        yield return new WaitForSeconds(2f);

        if (tip != 0)
        {
            GameObject tipPrefab = Instantiate(bonusPrefab, localMoneyText.transform.position - Vector3.up * 150f, timerText.transform.parent.rotation, gameCanvas.transform);
            tipPrefab.GetComponent<MoveIUElement>().SetDestination(localMoneyText.transform.position);
            tipPrefab.GetComponent<AssignBonusText>().SetBonusText(tipMoneyString + " ", Mathf.FloorToInt(tip).ToString());
        }
        yield return null;
    }

    public void AddMoney(int fee, int tip)
    {
        StartCoroutine(DoubleBonusPrefab(fee, tip));
        localMoney += fee + tip;
    }

    public void AddTime(float addingTime)
    {
        GameObject temp = Instantiate(bonusPrefab, timerText.transform.parent.position - Vector3.up*150f, timerText.transform.parent.rotation, gameCanvas.transform);
        temp.GetComponent<MoveIUElement>().SetDestination(timerText.transform.parent.position);
        temp.GetComponent<AssignBonusText>().SetBonusText(bonusTimeString + " ", Mathf.FloorToInt(addingTime).ToString());
        //addedTimeText.gameObject.SetActive(true);
        //addedTimeText.SetText("+" + Mathf.FloorToInt(addingTime).ToString());
        timeRemaining += addingTime;
        //StartCoroutine(FadeOut(addedTimeText.gameObject, 4f));
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
        totalText.SetText("Earned money: " + localMoney.ToString() + "\nTotal delivery: " + countDelivery.ToString());
        //totalText.SetText("Total delivery: " + countDelivery.ToString() + "\nEarned money: "+ localMoney.ToString());
        player.globalMoney += localMoney;
        if (localMoney > player.record)
            player.record = localMoney;

        SavePlayer();
    }

    public void OpenOptions()
    {
        optionsCamera.Priority = 1;
        menuCamera.Priority = 0;
        optionsCanvas.SetActive(true);
        startCanvas.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsCamera.Priority = 0;
        menuCamera.Priority = 1;
        optionsCanvas.SetActive(false);
        startCanvas.SetActive(true);
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
        //addedTimeText.gameObject.SetActive(false);
        inPlay = true;
        gameHasEnded = false;
        mainCamera.Priority = 1;
        menuCamera.Priority = 0;

        gameCanvas.SetActive(true);
        startCanvas.SetActive(false);

        //localMoneyText.transform.parent.gameObject.SetActive(false);
        recordText.gameObject.SetActive(false);

        localMoney = 0;

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
        int money = Mathf.FloorToInt(feeForDeliveryFor100Metres*(tempRestaurant.GetComponent<ClienteAssegnato>().GetDistanceFromClient())/100);
        deliveryTimeCanvas.gameObject.SetActive(false);
        if (deliveryTimeRemaining > deliveryTime * minSpeedyBonus)
        {
            AddTime(speedyTimeBonus);
            AddMoney(money, speedyTip);
            SetColorAndTextAndThenFadeOut(messageText, Color.green, speedyText);
        }
        else if (deliveryTimeRemaining > deliveryTime * minNormalBonus)
        {
            AddTime(normalTimeBonus);
            AddMoney(money, normalTip);
            SetColorAndTextAndThenFadeOut(messageText, Color.yellow, normalText);
        }
        else if (done)
        {
            AddMoney(money, 0);
            SetColorAndTextAndThenFadeOut(messageText, Color.red, badText);
        }else
        {
            SetColorAndTextAndThenFadeOut(messageText, Color.red, missedText);
        }

        client.SetActive(false);
        if(done)
            countDelivery++;

        DeAssignClient();
        AssignClient();
        ReactivateAll(restaurants);

        if (localMoney > player.record)
            recordText.gameObject.SetActive(true);
        if (localMoney > 0)
            localMoneyText.transform.parent.gameObject.SetActive(true);

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
        player = GameObject.Find("Motorino").GetComponent<Player>();

        LoadPlayer();

        timeRemaining = gameTime;
        PopulateList(restaurantsParent.GetComponentsInChildren<Transform>(), restaurants);
        PopulateList(clientsParent.GetComponentsInChildren<Transform>(), clients);

        DeactivateAll(clients);
        AssignClient();

        arrow.SetActive(false);
        deliveryTimeCanvas.transform.SetParent(GameObject.Find("Portacibo").transform);
        deliveryTimeCanvas.SetActive(false);
        messageText.gameObject.SetActive(false);

        if (!reloaded)
        {
            menuCamera.Priority = 1;
            mainCamera.Priority = 0;

            pauseCanvas.SetActive(false);
            gameCanvas.SetActive(false);
            gameOverCanvas.SetActive(false);
            optionsCanvas.SetActive(false);
            startCanvas.SetActive(true);
        }
        else
        {
            gameOverCanvas.SetActive(false);
            pauseCanvas.SetActive(false);
            optionsCanvas.SetActive(false);
            startCanvas.SetActive(false);
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

        if(localMoneyText.gameObject.activeSelf)
            localMoneyText.SetText(localMoney.ToString());
        if(globalMoneyText.gameObject.activeSelf)
            globalMoneyText.SetText(player.globalMoney.ToString());
    }
}
