using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManagement : MonoBehaviour {
	public Text challengeBest;
	public Text accuracyBest;
	public Text speedBest;

	public Text scoreText;
	public Text score2Text;
	public Text streakText;
	public Text timerText;

	GameObject GameManager;
	GameManagement GameManagement;

	public GameObject menu;
	public GameObject replayMenu;
	public GameObject hud;
	public GameObject hud2;
	public GameObject timer;
	public GameObject newHighScore;
	public GameObject timesUp;
	public GameObject wrongButton;
	public GameObject miss;
	public GameObject credits;

	public Image extraTime;

	public GameObject replayButton;
	public GameObject homeButton;
	public GameObject shareButton;

	// Use this for initialization
	void Start () {
		GameManager = GameObject.Find ("GameManager");
		GameManagement = GameManager.GetComponent<GameManagement>();

		NewMenu ();

		UpdateHighScores();
	}

	void Update(){
		scoreText.text = GameManagement.score.ToString ();
		score2Text.text = GameManagement.score.ToString ();
		streakText.text = GameManagement.streak.ToString ();
		timerText.text = GameManagement.timer.ToString ("0.00");

		//Check for a high score
		if (GameManagement.newHigh){
			if (GameManagement.showTxt)
				newHighScore.SetActive(true);
			else
				newHighScore.SetActive(false);
		}

		//Check the different lose conditions
		if (GameManagement.hitButton){
			if (GameManagement.showTxt)
				if (wrongButton != null)
					wrongButton.SetActive(true);
			else
				if (wrongButton != null)
					wrongButton.SetActive(false);
		}else if (GameManagement.timesUp){
			if (GameManagement.showTxt)
				if (timesUp != null)
					timesUp.SetActive(true);
			else
				if (timesUp != null)
					timesUp.SetActive(false);
		}else if (GameManagement.missed){
			if (GameManagement.showTxt)
				if (miss != null)
					miss.SetActive(true);
			else
				if (miss != null)
					miss.SetActive(false);
		}
	}

	public void UpdateHighScores(){
		challengeBest.text = PlayerPrefs.GetInt ("best").ToString ();
		accuracyBest.text = PlayerPrefs.GetInt ("accuracyBest").ToString ();
		speedBest.text = PlayerPrefs.GetInt ("speedBest").ToString ();
	}

	public void NewMenu(){
		if (GameManagement.showMenu){ //On Menu
			menu.SetActive (true);
			replayMenu.SetActive (false);
			hud.SetActive (false);
			hud2.SetActive (false);
			timer.SetActive (false);
			wrongButton.SetActive(false);
			timesUp.SetActive(false);
			miss.SetActive(false);
			newHighScore.SetActive (false);
		}else{
			menu.SetActive (false);
			timer.SetActive (true);
			if (GameManagement.gameMode == 0)
				hud.SetActive (true);
			else{
				hud2.SetActive (true);
			}
			if (GameManagement.showReplayScreen){ //Choose if you want to replay
				replayMenu.SetActive (true);
				StartCoroutine (EnableReplayButtons());
			}else{ //On Replay
				replayMenu.SetActive (false);
				wrongButton.SetActive(false);
				timesUp.SetActive(false);
				miss.SetActive(false);
				newHighScore.SetActive (false);
			}
		}
	}

	//Alternate the replay buttons from no interactable to interactable
	public IEnumerator EnableReplayButtons(){
		replayButton.GetComponent<Button>().interactable = false;
		homeButton.GetComponent<Button>().interactable = false;
		shareButton.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(.65f);
		replayButton.GetComponent<Button>().interactable = true;
		homeButton.GetComponent<Button>().interactable = true;
		shareButton.GetComponent<Button>().interactable = true;

	}

	public void ShowGames(){
		if (Application.platform == RuntimePlatform.Android){
			Application.OpenURL ("https://play.google.com/store/apps/developer?id=Luna+Wolf+Studios");
		}else{ //Do iOS Stuff
			Application.OpenURL ("https://itunes.apple.com/us/artist/luna-wolf-studios/id988182306");
		}
	}

	public void ShowTwitter(){
		Application.OpenURL ("https://twitter.com/lunawolfstudios");
	}

	public void ShowCredits(){
		menu.SetActive (false);
		credits.SetActive (true);
	}

	public void CloseCredits(){
		credits.SetActive (false);
		menu.SetActive (true);
	}
	
	public void ShowExtraTime() {
		extraTime.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		StartCoroutine (FadeExtraTime());
	}

	//Reduce the timers transparency overtime till its gone
	IEnumerator FadeExtraTime(){
		for (int i = 0; i < 10; i++){
			float tempTransparency = extraTime.color.a;
			tempTransparency -= .10f;
			extraTime.color = new Color (1.0f, 1.0f, 1.0f, tempTransparency);
			yield return new WaitForSeconds(.25f);
		}
	}

	/*Posting to Twitter and Facebook is done below */
	/*private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
	private const string TWEET_LANGUAGE = "en";
	
	public void ShareToTwitter (){
		Application.OpenURL(TWITTER_ADDRESS +
                "?text=" + WWW.EscapeURL("I just scored " +
              	PlayerPrefs.GetInt ("challengeBest").ToString()) +
                " on Target Crack. Try to beat me! It's free to play! %23TargetCrack @LunaWolfStudios " +
                "&amp;lang=" + WWW.EscapeURL(TWEET_LANGUAGE));
	}


	private const string FACEBOOK_APP_ID = "626239597520980"; //Target Crack App ID as specified through FaceBook
	private const string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";
	
	 public void ShareToFaceBook (){
		Application.OpenURL (FACEBOOK_URL + "?app_id=" + FACEBOOK_APP_ID +
	             "&link=" + WWW.EscapeURL("http://lunawolfstudios.com") +
	             "&name=" + WWW.EscapeURL("Target Crack") +
	             "&caption=" + WWW.EscapeURL("Target Crack") + 
	             "&description=" + WWW.EscapeURL("I just scored " +
	          	 PlayerPrefs.GetInt ("challengeBest").ToString() +
	         	 " on Target Crack. Try to beat me! It's free to play! https://twitter.com/lunawolfstudios #TargetCrack ") + 
	             //"&picture=" + WWW.EscapeURL()
				"&redirect_uri=" + WWW.EscapeURL("http://www.facebook.com/"));
	}*/
}
