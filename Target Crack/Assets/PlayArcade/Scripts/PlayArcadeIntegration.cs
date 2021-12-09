using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class PlayArcadeIntegration : MonoBehaviour
{
    
    public PlayArcade playArcade;
    public GameObject gamePanel;

    public GameObject startGamePanel;
    public Text startGameMessageText;
    public GameObject startGameButton;
    public Text startGameButtonCost;
    
    public GameObject inGamePanel;
    public GameObject leaderBoard;
    public Text playerNameText;
    public Text buyMessageText;
    
    [Tooltip("TEST_USER_ID can be your developer ID from the PlayArcade Dev Console https://theplayarcade.com/developer/games")]
    public string TEST_USER_ID = "";
    [Tooltip("From the PlayArcade Dev Console")]
    public string GAME_ID = ""; 
    [Tooltip("From the PlayArcade Dev Console")]
    public string API_KEY = "";
    [Tooltip("Experimental")]
    public string TEST_USER_NAME = "Player"; 
    [Tooltip("Use PLAY coin for testing!")]
    public string COIN_KIND = "PLAY";
    public bool freePlayerTest = false;
    public bool devBuild = false;
    public bool isInitialized = false;
    public string sPlayerName = "";
    private string k1,k2,k3,k4,k5;
    private int player_score = 0;

    private JSONObject gameCustomizationData;
    private JSONObject storeData;
    private JSONObject coinScoreData;
    private JSONObject userCoinScoreData;
    public List <PlayArcadeStoreItem> StoreItems = new List<PlayArcadeStoreItem>();
    public List <PlayArcadeGameCustomization> GameCustomizations = new List<PlayArcadeGameCustomization>();
    public List <PlayArcadeScoreItem> CoinScores = new List<PlayArcadeScoreItem>();
    public List <PlayArcadeScoreItem> UserCoinScores = new List<PlayArcadeScoreItem>();
    
    public AudioSource tstAudioSource;
    // Coins exist in every game, so we can grab them here
    [Tooltip("This matches what you've set in your customizations online")]
    public string coinName = "coin_image";
    public Image coinImage;
    public Image savedCoinImage;
    public Texture2D coinTexture;
    public Material coinMaterial;
    [Tooltip("This matches what you've set in your customizations online")]
    public string avatarURLName = "MyReadyPlayerMeURL";
    public string AvatarURL = "https://d1a370nemizbjq.cloudfront.net/47422f7c-4fa9-4a35-90f2-637b3731dcb6.glb";
    public int numScoresToRetrieve = 10;
    public int lastScore = 0;
    
    // Current instance management
    private static PlayArcadeIntegration _current;
    public static PlayArcadeIntegration Instance
    {
        get
        {
            if(_current == null)
                Debug.LogError("Attempting to access PlayArcadeIntegration before it's been created.");
            return _current;
        }
    }
 
    void Awake ()
    {
        if (_current != null)
        {
            DestroyImmediate(_current.gameObject);
            //Debug.LogError("More than one copy of PlayArcadeIntegration exists.");
        }

        _current = this;
    }
 
    void OnDestroy ()
    {
        _current = null;
    }
    
    void Start()
    {
        if (isInitialized)
            return;

        // gamePanel.SetActive(false);
        // inGamePanel.SetActive(false);
        // startGameButton.SetActive(false);
        // startGamePanel.SetActive(false);

        //playerNameText.text = sPlayerName;
        StartPlayArcade();
        isInitialized = true;
    }

    public void Update()
    {
        if (playArcade == null)
            playArcade = this.GetComponent<PlayArcade>();
    }

    private void StartPlayArcade()
    {
        if(playArcade == null){
            Debug.LogError("playArcade Object is not set");
            return;
        } 
        if(GAME_ID == ""){
            Debug.LogError("You must set the GAME_ID variable in PlayArcade class object.");
            return;
        }
        if(API_KEY == ""){
            Debug.LogError("You must set the API_KEY variable in PlayArcade class object.");
            return;
        }
        if (freePlayerTest)
            TEST_USER_ID = "free"; //For testing free game functionality (limited functionality)        

        playArcade.pa_OnAppStarted = OnAppStarted;
        playArcade.pa_EnableStartButton = EnableStartButton;
        playArcade.pa_StartGameSession = StartGameSession;
        playArcade.pa_SessionUpdated = OnSessionUpdated;
        playArcade.pa_SessionEventSent = OnSessionEventSent;
        playArcade.pa_ScoreSubmitted = OnScoreSubmitted;
        
        if (devBuild == false)
        {
            playArcade.Init(GAME_ID, API_KEY, COIN_KIND);
        }
        else
        {
            playArcade.Init(GAME_ID, API_KEY, COIN_KIND, TEST_USER_ID, TEST_USER_NAME);
        }
        // This is the first log you should be seeing
        Debug.Log("Asking for initialization.");
    }

    // Play Arcade is fully initialized, we can begin reading data from the server
    private void OnAppStarted(bool success, string _sPlayerName, JSONObject _gameInfo){
        sPlayerName = _sPlayerName;
        Debug.Log("PlayArcade is fully initialized for " + _sPlayerName);

        if(success)
        {
            //Debug.Log(_gameInfo.ToString());
            //GetHighScores();
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
        }
    }

    private void MainMenu()
    {
        DownloadMaterialTexture(coinName,coinTexture,coinMaterial);
        // playerNameText.text = "Welcome, " + sPlayerName;
        // gamePanel.SetActive(true);
        // startGamePanel.SetActive(true);
        EnableStartButton(true);
    }

    private void EnableStartButton(bool state, string message="")
    {
        // button could be null if we entered here from a non-mainmenu
        if (startGameButton == null) return;
        //startGameButton.SetActive(state);
        startGameMessageText.text = message;
    }

    //Point your Start/Play button here to kick it off! Named Button<> for easier finding in the dropdown list
    public void ButtonStartGameRequest()
    {
        playArcade.StartGameRequest();
    }

    //This is the callback from the StartGameRequest
    private void StartGameSession(string sk1, string sk2, string sk3, string sk4, string sk5){
        
        startGamePanel.SetActive(false);
        k1=sk1;k2=sk2;k3=sk3;k4=sk4;k5=sk5;
        string someSessionDescription = "Level 1";
        playArcade.SendSessionStart(someSessionDescription, k1);
    }

    public void EndGameTest(){
        //Sample Call to End the Session
        player_score = 10901;
        SubmitScore(player_score, "This is awesome");
    }

    public void SendGameEventTest(){
        SendGameEvent("Boss Defeated", sPlayerName + " Defeated the Ogre!", k3, 0f);
    }
    public void SendGameEventCredit(){
        SendGameEvent("credit_bag", sPlayerName, k3, 0f);
    }
    private void SendGameEvent(string eventName, string eventDetails, string skey, float delay){
        //skey should be the k3 key which was received when the session began
        playArcade.SendSessionEvent(eventName, eventDetails, skey, 0);
    }   

    //When player is done with round, send in the score
    public void SubmitScore(int score, string sessionStats="")
    {
        //Must send Update, Score, Stats, End.
        playArcade.SendSessionScore(score, k3, 0);
        playArcade.SendSessionStats(sessionStats, k4, 0);
        playArcade.SendSessionUpdate(score, k2, 0);
    }

    private void OnSessionUpdated(){
        string msg = "Yes!";
        playArcade.SendSessionEnd(msg, k5, 0);
    }

    private void OnScoreSubmitted(){
        Debug.Log("Score saved.. do something else!");
        // inGamePanel.SetActive(false);
        // startGamePanel.SetActive(false);
        // StartPlayArcade();
    }

    private void OnSessionEventSent(string eventName){
        Debug.Log("Session Event Sent!: " + eventName);
        
    }

    //Customizations
    public void GetCustomizations(){
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
    
    private void processCustomizations(){
        GameCustomizations.Clear();
        JSONObject jItems = gameCustomizationData;
        Debug.Log(jItems.ToString());
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
            
            if (customization_name == avatarURLName)
                AvatarURL = customization_value;
        }
        Debug.Log("Number Game Customizations retrieved:" + GameCustomizations.Count());
        //AvatarLoader avatarLoader = new AvatarLoader();
        // some other file can deal with results of the loaded avatar. It must contain:
        // public void OnAvatarImported(GameObject avatar)
        // {
        //     Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
        // }
        //
        // public void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
        // {
        //     Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");
        // }

        // LoadoutState manager = GameObject.Find ("Loadout").GetComponent<LoadoutState> ();
        // avatarLoader.LoadAvatar(PlayArcadeIntegration.Instance.AvatarURL, manager.OnAvatarImported, manager.OnAvatarLoaded);

    }

    public void TestTextureCustomization()
    {
        StartCoroutine(GetCustomTexture(coinName, coinImage));
        savedCoinImage = coinImage;
    }

    public void DownloadMaterialTexture(string textureName, Texture2D targetObject, Material targetMaterial)
    {
        StartCoroutine(GetMaterialTexture(textureName, targetObject, targetMaterial));
    }
    public void testAudioCustomization(){
        //StartCoroutine(GetCustomAudio("sound1", tstFaceImage));
    }
    private IEnumerator GetMaterialTexture(string resourceName, Texture2D targetObject, Material targetMaterial) {
        if (targetMaterial == null || targetObject == null)
            print("Danger Will Robinson!");
        PlayArcadeGameCustomization theCustomization = GameCustomizations.Find(x => x.customization_name.ToLower() == resourceName.ToLower());
        string theURL = theCustomization.customization_value;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(theURL);
        yield return www.SendWebRequest();
        
        Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture;
        targetMaterial.mainTexture = tex;
    }
    
    private IEnumerator GetCustomTexture(string resourceName, Image targetObject) {
        PlayArcadeGameCustomization theCustomization = GameCustomizations.Find(x => x.customization_name.ToLower() == resourceName.ToLower());
        string theURL = theCustomization.customization_value;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(theURL);
        yield return www.SendWebRequest();
        
        Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2f, tex.height / 2f));

        if (targetObject != null)
            targetObject.sprite = sprite;
    }
    
    private IEnumerator GetCustomSprite(string resourceName, SpriteRenderer targetObject) {
        PlayArcadeGameCustomization theCustomization = GameCustomizations.Find(x => x.customization_name.ToLower() == resourceName.ToLower());
        string theURL = theCustomization.customization_value;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(theURL);
        yield return www.SendWebRequest();
        
        Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        if (targetObject != null)
            targetObject.sprite = sprite;
    }

    IEnumerator GetCustomAudio(string resourceName, Image targetObject)
    {
        PlayArcadeGameCustomization theCustomization = GameCustomizations.Find(x => x.customization_name == resourceName);
        string theURL = theCustomization.customization_value;
    
        using (var www = new WWW(theURL))
        {
            yield return www;
            tstAudioSource.clip = www.GetAudioClip();
            if (!tstAudioSource.isPlaying && tstAudioSource.clip.loadState == AudioDataLoadState.Loaded)
                tstAudioSource.Play();
        }
    }

    #region IAP
    
    //IAP Samples
    public void GetStoreData(){
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

    private void processStoreItems(){
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
            if(application_credit_item_id=="game_start"){
                // startGameButton.SetActive(true);
                if (item_cost <= 0)
                    startGameButtonCost.text = "Free";
                else
                    startGameButtonCost.text = item_cost.ToString() + " Credit(s)";
            }
        }
        Debug.Log("Number store items retrieved:" + StoreItems.Count());
    }

    public void BuyAThing(string thingName){
        playArcade.BuyCreditItem(thingName, OnTestBuyItemComplete);
    }

    private void OnTestBuyItemComplete(bool success, string application_credit_item_id, string message)
    {
        if(buyMessageText) buyMessageText.text = message;
        if (success)
        {
            Debug.Log("Purchase of " + application_credit_item_id + " successful! " + message);
        }
        else
        {
            Debug.Log("Purchase of " + application_credit_item_id + " failed! " + message);
        }
    }
    #endregion
        // Single player
    private void processCoinScores()
    {
        CoinScores.Clear();
        JSONObject jScoreItems = coinScoreData; //.GetField("store_items");
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
            CoinScores.Add(newScoreItem);
            if (score > lastScore)
                lastScore = score;
            Debug.Log("Score: " + score + "User: "+user_name);
        }
        //Debug.Log("Number coin scores:" + CoinScores.Count());
    }
    
        // Leaderboard
    private void processUserCoinScores(){
         UserCoinScores.Clear();
         JSONObject jScoreItems = userCoinScoreData; //.GetField("store_items");
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
             UserCoinScores.Add(newScoreItem);
             //Debug.Log("Score Item:" + application_credit_item_id + " Cost: " + item_cost.ToString());
         }
         Debug.Log("Number user coin scores:" + UserCoinScores.Count());
     }

    public void GetHighScores(){
        playArcade.GetHighScores(OnHighScoreDataRetrieved, numScoresToRetrieve);
    }

    private void OnHighScoreDataRetrieved(bool success, JSONObject _scoreData)
    {
        if (success)
        {
            Debug.Log("HighScore data retrieved!");
            JSONObject jScores = _scoreData.GetField("gamePublisherScores");
            coinScoreData = jScores;
            processCoinScores();
        }
        else
        {
            Debug.Log("Error fetching Score Data");
        }
    }
}
