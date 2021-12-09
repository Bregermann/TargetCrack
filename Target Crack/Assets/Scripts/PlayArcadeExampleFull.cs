using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using BestHTTP;

public class PlayArcadeExampleFull : MonoBehaviour
{
    public PlayArcade playArcade;
    public string GAME_ID; //From the PlayArcade Dev Console
    public string API_KEY; //From the PlayArcade Dev Console
    public bool DevMode = true;
    public GameObject gamePanel;

    public GameObject startGamePanel;
    public Text startGameMessageText;
    public GameObject startGameButton;
    public Text startGameButtonCost;

    public GameObject inGamePanel;
    public Text playerNameText;
    public Text buyMessageText;

    private string COIN_KIND = "";
    private string sPlayerName = "";
    private string k1, k2, k3, k4, k5;
    private int player_score = 0;

    private JSONObject gameCustomizationData;
    private JSONObject storeData;
    private JSONObject coinScoreData;
    private JSONObject userCoinScoreData;
    private List<PlayArcadeStoreItem> StoreItems = new List<PlayArcadeStoreItem>();
    private List<PlayArcadeGameCustomization> GameCustomizations = new List<PlayArcadeGameCustomization>();
    private List<PlayArcadeScoreItem> CoinScores = new List<PlayArcadeScoreItem>();
    private List<PlayArcadeScoreItem> UserCoinScores = new List<PlayArcadeScoreItem>();

    public Image tstFaceImage;
    public AudioSource tstAudioSource;

    private void Start()
    {
        DontDestroyOnLoad(playArcade);
        gamePanel.SetActive(false);
        inGamePanel.SetActive(false);
        startGameButton.SetActive(false);
        startGamePanel.SetActive(false);
        playerNameText.text = sPlayerName;
        StartPlayArcade();
    }

    private void StartPlayArcade()
    {
        playArcade.pa_OnAppStarted = OnAppStarted;
        playArcade.pa_EnableStartButton = EnableStartButton;
        playArcade.pa_StartGameSession = StartGameSession;
        playArcade.pa_SessionUpdated = OnSessionUpdated;
        playArcade.pa_SessionEventSent = OnSessionEventSent;
        playArcade.pa_ScoreSubmitted = OnScoreSubmitted;

        COIN_KIND = "PLAY"; //Use PLAY coin for testing!

        if (playArcade == null)
        {
            Debug.LogError("playArcade Object is not set");
            return;
        }

        if (GAME_ID == "")
        {
            Debug.LogError("GAME_ID variable not set.");
            return;
        }

        if (API_KEY == "")
        {
            Debug.LogError("API_KEY variable not set.");
            return;
        }

        //When Done, the pa_OnAppStarted callback is fired off
        if (!DevMode)
        {
            playArcade.Init(GAME_ID, API_KEY, COIN_KIND);
        }
        else
        {
            //USE THIS INIT CALL FOR DEV TESTING
            Debug.LogWarning("Running Game In DEV MODE");

            string TEST_USER_ID = "29ef7b62-9f25-4622-bf92-04cd0d553f63"; //Your developer ID from the PlayArcade Dev Console https://theplayarcade.com/developer/games
            string TEST_PUBLISHER_ID = ""; //A publisher to test with. Play's publisher ID is: 527b1b29-67f3-4bca-b6d3-8858ad53b0e2
            //string TEST_USER_ID = "free"; //For testing free game functionality (experimental)
            string TEST_USER_NAME = "Player"; //Experimental
            playArcade.Init(GAME_ID, API_KEY, COIN_KIND, TEST_USER_ID, TEST_USER_NAME, TEST_PUBLISHER_ID);
        }
    }

    private void OnAppStarted(bool success, string _sPlayerName, string _sCoinKind, JSONObject _gameInfo)
    {
        sPlayerName = _sPlayerName;
        COIN_KIND = _sCoinKind;
        //Debug.Log("OnAppStarted " + _sPlayerName);

        if (success)
        {
            //Debug.Log(_gameInfo.ToString());
            JSONObject jLocalTest = _gameInfo.GetField("local_test");
            if (jLocalTest)
            {
                playArcade.doLocalTest(true);
            }

            //Process Store Data if present
            JSONObject jStoreData = _gameInfo.GetField("store_items");
            storeData = jStoreData;
            processStoreItems();

            //Process Customization Data if present
            JSONObject jCustomizationData = _gameInfo.GetField("customizations");
            gameCustomizationData = jCustomizationData;
            processCustomizations();

            //Process Score Data if present
            JSONObject jCoinScoreData = _gameInfo.GetField("game_scores_coin");
            coinScoreData = jCoinScoreData;
            processCoinScores();

            //Process Score Data if present
            JSONObject jUserScoreData = _gameInfo.GetField("game_scores_user");
            userCoinScoreData = jUserScoreData;
            processUserCoinScores();

            MainMenu();

            GetHighScores();
        }
    }

    private void MainMenu()
    {
        playerNameText.text = "Welcome, " + sPlayerName;
        gamePanel.SetActive(true);
        startGamePanel.SetActive(true);
    }

    private void EnableStartButton(bool enabled, string message = "")
    {
        startGameButton.SetActive(enabled);
        startGameMessageText.text = message;
    }

    //Point your Start/Play button here to kick it off!
    public void StartGameRequest()
    {
        playArcade.StartGameRequest();
    }

    //This is the callback from the StartGameRequest
    private void StartGameSession(string sk1, string sk2, string sk3, string sk4, string sk5)
    {
        startGamePanel.SetActive(false);
        inGamePanel.SetActive(true);
        k1 = sk1; k2 = sk2; k3 = sk3; k4 = sk4; k5 = sk5;
        string someSessionDescription = "Level 1";
        playArcade.SendSessionStart(someSessionDescription, k1);
    }

    public void EndGameTest()
    {
        //Sample Call to End the Session
        player_score = 10901;
        SubmitScore(player_score, "This is awesome");
    }

    public void SendGameEventTest()
    {
        SendGameEvent("warren_enter_friendly", sPlayerName + " from $" + COIN_KIND + " has entered the $" + COIN_KIND + " warren. Fear not, little ones, for they are a friend indeed.", k3, 0f);
    }

    private void SendGameEvent(string eventName, string eventDetails, string skey, float delay)
    {
        //skey should be the k3 key which was received when the session began
        playArcade.SendSessionEvent(eventName, eventDetails, skey, 0);
    }

    //When player is done with round, send in the score
    private void SubmitScore(int score, string sessionStats = "")
    {
        //Must send Update, Score, Stats, End.
        playArcade.SendSessionScore(score, k3, 0);
        playArcade.SendSessionStats(sessionStats, k4, 0);
        playArcade.SendSessionUpdate(score, k2, 0);
    }

    private void OnSessionUpdated()
    {
        string msg = "Yes!";
        playArcade.SendSessionEnd(msg, k5, 0);
    }

    private void OnScoreSubmitted()
    {
        Debug.Log("Score saved.. do something else!");
        inGamePanel.SetActive(false);
        startGamePanel.SetActive(false);
        StartPlayArcade();
    }

    private void OnSessionEventSent(string eventName)
    {
        Debug.Log("Session Event Sent!: " + eventName);
    }

    //Customizations
    public void GetCustomizations()
    {
        playArcade.GetGameCustomizations(OnCustomizationsRetrieved);
    }

    private void OnCustomizationsRetrieved(bool success, JSONObject _gameCustomizations)
    {
        if (success)
        {
            Debug.Log("Game customizations retrieved!");
            gameCustomizationData = _gameCustomizations;
            processCustomizations();
        }
        else
        {
            Debug.Log("Error fetching game customizations");
        }
    }

    private void processCustomizations()
    {
        GameCustomizations.Clear();
        JSONObject jItems = gameCustomizationData; //.GetField("customizations");
        foreach (JSONObject jItem in jItems.list)
        {
            string item_id = jItem.GetField("id").str;
            string customization_id = jItem.GetField("id").str;
            string customization_base = jItem.GetField("base").str;
            string customization_name = jItem.GetField("name").str;
            string customization_type = jItem.GetField("type").str;
            string customization_value = jItem.GetField("value").str;
            PlayArcadeGameCustomization newCustomization = new PlayArcadeGameCustomization(customization_id, customization_base, customization_name, customization_type, customization_value);
            GameCustomizations.Add(newCustomization);
        }
        Debug.Log("Number Game Customizations retrieved:" + GameCustomizations.Count());
    }

    public void testTexureCustomization()
    {
        StartCoroutine(GetCustomTexture("head1", tstFaceImage));
    }

    private IEnumerator GetCustomTexture(string resourceName, Image targetObject)
    {
        PlayArcadeGameCustomization theCustomization = GameCustomizations.Find(x => x.customization_name.ToLower() == resourceName.ToLower());
        string theURL = theCustomization.customization_value;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(theURL);
        yield return www.SendWebRequest();

        //Texture myTexture = DownloadHandlerTexture.GetContent(www);
        Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        targetObject.GetComponent<Image>().overrideSprite = sprite;
    }

    public void testAudioCustomization()
    {
        StartCoroutine(GetCustomAudio("sound1", tstFaceImage));
    }

    private IEnumerator GetCustomAudio(string resourceName, Image targetObject)
    {
        PlayArcadeGameCustomization theCustomization = GameCustomizations.Find(x => x.customization_name == resourceName);
        string theURL = theCustomization.customization_value;

        using (var www = new WWW(theURL))
        {
            yield return www;
            tstAudioSource.clip = www.GetAudioClip();
            if (!tstAudioSource.isPlaying && tstAudioSource.clip.isReadyToPlay)
                tstAudioSource.Play();
        }
    }

    //IAP Samples
    public void GetStoreData()
    {
        playArcade.GetStoreData(OnStoreDataRetrieved);
    }

    private void OnStoreDataRetrieved(bool success, JSONObject _storeData)
    {
        if (success)
        {
            Debug.Log("Store Data retrieved!");
            storeData = _storeData;
            processStoreItems();
        }
        else
        {
            Debug.Log("Error fetching Store Data");
        }
    }

    private void processStoreItems()
    {
        StoreItems.Clear();
        JSONObject jStoreItems = storeData; //.GetField("store_items");
        //Debug.Log(jStoreItems.ToString());
        foreach (JSONObject jStoreItem in jStoreItems.list)
        {
            string item_id = jStoreItem.GetField("id").str.ToLower();
            string application_credit_item_id = jStoreItem.GetField("application_credit_item_id").str;
            string friendly_name = jStoreItem.GetField("friendly_name").str;
            string description = jStoreItem.GetField("description").str;
            int item_cost = (int)jStoreItem.GetField("item_cost").i;
            string display_order = jStoreItem.GetField("display_order").str;
            string tags = jStoreItem.GetField("tags").str;
            PlayArcadeStoreItem newStoreItem = new PlayArcadeStoreItem(item_id, application_credit_item_id, friendly_name, description, item_cost, display_order, tags);
            StoreItems.Add(newStoreItem);

            //Debug.Log("Store Item:" + application_credit_item_id + " Cost: " + item_cost.ToString());
            //Use the game start item to set how much it costs to play...
            if (application_credit_item_id == "game_start")
            {
                startGameButton.SetActive(true);
                startGameButtonCost.text = item_cost.ToString() + " Credit(s)";
            }
        }
        Debug.Log("Number store items retrieved:" + StoreItems.Count());
    }

    public void BuyAThing(string thingName)
    {
        playArcade.BuyCreditItem(thingName, OnTestBuyItemComplete);
    }

    private void OnTestBuyItemComplete(bool success, string application_credit_item_id, string message)
    {
        if (buyMessageText) buyMessageText.text = message;
        if (success)
        {
            Debug.Log("Purchase of " + application_credit_item_id + " successful! " + message);
        }
        else
        {
            Debug.Log("Purchase of " + application_credit_item_id + " failed! " + message);
        }
    }

    public void GetHighScores()
    {
        playArcade.GetHighScores(OnHighScoreDataRetrieved, 25);
    }

    private void OnHighScoreDataRetrieved(bool success, JSONObject _scoreData)
    {
        if (success)
        {
            Debug.Log("Score Data retrieved!");
            JSONObject jScores = _scoreData.GetField("gamePublisherScores");
            coinScoreData = jScores;
            processCoinScores();
        }
        else
        {
            Debug.Log("Error fetching Score Data");
        }
    }

    private void processCoinScores()
    {
        CoinScores.Clear();
        JSONObject jScoreItems = coinScoreData; //.GetField("store_items");
        //Debug.Log(jScoreItems.ToString());
        foreach (JSONObject jScoreItem in jScoreItems.list)
        {
            string game_session_id = jScoreItem.GetField("game_session_id").str.ToLower();
            string coin_kind = jScoreItem.GetField("coin_kind").str;
            string stats = jScoreItem.GetField("stats").str;
            string message = jScoreItem.GetField("message").str;
            int score = (int)jScoreItem.GetField("score").i;
            string publisher_id = jScoreItem.GetField("publisher_id").str;
            string user_name = jScoreItem.GetField("user_name").str;
            string user_id = jScoreItem.GetField("user_id").str;
            string date_created = jScoreItem.GetField("date_created").str;
            PlayArcadeScoreItem newScoreItem = new PlayArcadeScoreItem(
                game_session_id,
                coin_kind,
                score,
                stats,
                message,
                publisher_id,
                user_name,
                user_id,
                date_created
            );
            CoinScores.Add(newScoreItem);

            //Debug.Log("Score Item:" + application_credit_item_id + " Cost: " + item_cost.ToString());
        }
        Debug.Log("Number coin scores:" + CoinScores.Count());
    }

    private void processUserCoinScores()
    {
        UserCoinScores.Clear();
        JSONObject jScoreItems = userCoinScoreData; //.GetField("store_items");
        //Debug.Log(jStoreItems.ToString());
        foreach (JSONObject jScoreItem in jScoreItems.list)
        {
            string game_session_id = jScoreItem.GetField("game_session_id").str.ToLower();
            string coin_kind = jScoreItem.GetField("coin_kind").str;
            string stats = jScoreItem.GetField("stats").str;
            string message = jScoreItem.GetField("message").str;
            int score = (int)jScoreItem.GetField("score").i;
            string publisher_id = jScoreItem.GetField("publisher_id").str;
            string user_name = jScoreItem.GetField("user_name").str;
            string user_id = jScoreItem.GetField("user_id").str;
            string date_created = jScoreItem.GetField("date_created").str;
            PlayArcadeScoreItem newScoreItem = new PlayArcadeScoreItem(
                game_session_id,
                coin_kind,
                score,
                stats,
                message,
                publisher_id,
                user_name,
                user_id,
                date_created
            );
            UserCoinScores.Add(newScoreItem);

            //Debug.Log("Score Item:" + application_credit_item_id + " Cost: " + item_cost.ToString());
        }
        Debug.Log("Number user coin scores:" + UserCoinScores.Count());
    }
}