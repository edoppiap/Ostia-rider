using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour
{
    [Header("Windows")]
    public GameObject nameWindow;
    public GameObject[] reactiveAfterUsername;

    [Header("Display name window")]
    public GameObject nameError;
    public TMP_InputField nameInput;
    public GameObject loginText;

    [Header("Leaderboard")]
    public GameObject rowPrefab;
    public Transform rowsParent;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void Login(string customId)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    public void inLoggin()
    {
        loginText.SetActive(false);
        nameWindow.SetActive(true);
        foreach (GameObject obj in reactiveAfterUsername)
        {
            obj.SetActive(false);
        }
    }

    public void loggato()
    {
        nameWindow.SetActive(false);
        loginText.SetActive(false);
        foreach (GameObject obj in reactiveAfterUsername)
        {
            obj.SetActive(true);
        }
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successfurl login/account create!");
        string name = null;
        if(result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (name == null)
        {
            inLoggin();
        }
        else
        {
            loggato();
        }
    }

    void OnError(PlayFabError error)
    {
        nameWindow.SetActive(true);
        nameError.SetActive(true);
        foreach (GameObject obj in reactiveAfterUsername)
        {
            obj.SetActive(false);
        }
        Debug.Log("Errore durante il login/accesso");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
        nameWindow.SetActive(false);
        foreach (GameObject obj in reactiveAfterUsername)
        {
            obj.SetActive(true);
        }
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "RiderScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leaderboard inviata con successo");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "RiderScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        Player player = gameManager.getPlayer();
        int count = result.Leaderboard.Count;
        int range = count > 4 ? 4 : count;

        foreach(Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard.GetRange(0, range))
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            TMP_Text[] texts = newGo.GetComponentsInChildren<TMP_Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();
        }
    }
}
