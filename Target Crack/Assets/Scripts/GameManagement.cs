using UnityEngine;

using System.Collections;
using UnityEngine.SocialPlatforms;

//Current iAd banner room 80 pixels
//Leaderboards
//extremeHighScore
//tapTestHighScores //Don't forget the S on this one
//accuracyTestHighScore //Not Implemented yet
//PlayerPrefs
//best
//speedBest
//accuracyBest
//totalBroke
//bestStreak

public class GameManagement : MonoBehaviour {
	//Used for GUI Scaling
	float native_width = 750.0f;
	float native_height = 1334.0f;
	public GUISkin countDownFont;

	public int gameMode = 0;
	public bool gameOver = true;

	public GameObject badButton;
    public GameObject goodButton;
	GameObject bgm;

	//Starting time
	float startTime = 7.0f;

	//Current Time
	public float timer;

	public int guiCountDown = -1;

	public bool newHigh = false;
	public bool showTxt = false;
	public bool timesUp = false;
	public bool missed = false;

	public bool showMenu = true;
	public bool showReplayScreen = false;

	public bool hitButton = false;

	public bool noMiss = false; //Used to detect if you can hit the black or not

    //Number of buttons to spawn
	int numBadButtons = 10;
    int numGoodButtons = 4;

	//Current score and hit streak
	public int score = 0;
	public int streak = 0;

	//Variable for when more time should be added
	int addMoreTimeGoal = 10;
	//Variable for how much time should be added;
	float additionalTime = 4.0f;

	void Awake(){
		//PlayerPrefs.SetInt ("best", 0);
		//We are designing the game specifically for 9:16 resolution so set its aspect to that every time
		Camera.main.aspect = 9.0f/16.0f;
		bgm = GameObject.Find ("BGM");
	}

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;

		//Sign player into gamecenter
		//Social.localUser.Authenticate (success =>{string userInfo = "Username: " + Social.localUser.userName + "\nUser ID: " + Social.localUser.id + "\nIsUnderage: " + Social.localUser.underage;});

        //Make text blink
		InvokeRepeating ("ShowText", 0, 1);
		InvokeRepeating ("PitchChecker", 0, .10f);
	}
    
    // Update is called once per frame
    void Update () {
        
        if (!gameOver && timer < .01f){
			timesUp = true;
			timer = 0.0f;
			GameOver (gameMode);
		}

		//Detect to see if it got tapped
		if (Input.touchCount > 0 && noMiss){ //Make sure there is a tap on the screen
			//Loop through all the touches
			for (int i = 0; i < Input.touchCount; i++){
				//if the touch phase is just now starting then check to see if you hit anything with it
				if (Input.GetTouch (i).phase == TouchPhase.Began){
					CheckHit (Input.GetTouch (i).position);
				}
			}
		}else if (Input.GetMouseButtonDown (0) && noMiss){ //If you tap and you can not miss check to see if you missed
			CheckHit (Input.mousePosition);
		}

	}

	//Method used to determine the pitch of the background music
	void PitchChecker() {
		if (guiCountDown > 0){
			bgm.GetComponent<AudioSource>().pitch = 0.0f;
		}else{
			if (timer > 0 && timer < 4.25f && !gameOver){
				bgm.GetComponent<AudioSource>().pitch = 1.45f;
				/*if (timer > 1){
					if (timer > 2){
						if (timer > 3){
							if (timer > 4){
								bgm.GetComponent<AudioSource>().pitch = 1.12f;
							}else
								bgm.GetComponent<AudioSource>().pitch = 1.24f;
						}else
							bgm.GetComponent<AudioSource>().pitch = 1.36f;
					}else
						bgm.GetComponent<AudioSource>().pitch = 1.48f;
				}else
					bgm.GetComponent<AudioSource>().pitch = 1.60f;*/
			}else if (timer > 4.25f && !gameOver){
				bgm.GetComponent<AudioSource>().pitch = 1.10f;
	        }else{
				if (bgm != null)
					bgm.GetComponent<AudioSource>().pitch = 1.0f;
			}
		}
	}

	//Counts the timer down
	void CountDown (){
		if (!gameOver)
			timer -= .01f;
		else
			CancelInvoke ("CountDown");
	}

	//Method used to make sure you hit something
	void CheckHit(Vector3 tempPos){
		Ray ray = Camera.main.ScreenPointToRay(tempPos); //Get a straight line from starting from the mouse position
		RaycastHit hit; //Create a struct named "hit" to receive information from a raycast
		if (Physics.Raycast(ray, out hit)) { //Returns true if the raycast intersected a collider and sets it to "hit"
			if (hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled){
				return;
			}
		}
		//If you missed in challenge mode then set streak to 0
		if (gameMode == 0){
			streak = 0;
			return;
		}
		missed = true;
		gameObject.GetComponent<AudioManager>().PlayMissed ();
		GameOver (gameMode);
	}

	//Method used to check if you hit a specific gameobject and returns a boolean
	public bool CheckHit(GameObject check, Vector3 hitPos){
		Ray ray = Camera.main.ScreenPointToRay(hitPos); //Get a straight line from starting from the hit position
		RaycastHit hit; //Create a struct named "hit" to receive information from a raycast
		if (Physics.Raycast(ray, out hit)) { //Returns true if the raycast intersected a collider and sets it to "hit"
			if (hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled && hit.collider.gameObject == check){
				return(true);
			}
		}
		return(false);
	}

	//Method used to check if a new high streak was earned
	void CheckNewStreak(){
		//Set a new highest streak if applicable
		if (streak > PlayerPrefs.GetInt ("bestStreak")){
			UM_GameServiceManager.instance.SubmitScore("Highest_Streak", streak);
			PlayerPrefs.SetInt ("bestStreak", streak);
		}
	}
	
	//Creates all the button objects depending on the game mode
	void GenerateButtons(int gameMode){
        if (gameMode == 0){
    		for (int i = 0; i < numBadButtons; i++){
    			Instantiate (badButton, new Vector3(0, 0, 0), badButton.transform.rotation);
    		}
            for (int i = 0; i < numGoodButtons; i++){
				Instantiate (goodButton, new Vector3(0, 0, 0), goodButton.transform.rotation);
            }
        }else if (gameMode == 1){ //Cover the screen in targets
			int rows = 6;
			int cols = 4;

			float xPos;
			float yPos = 5.0f;
			float shiftX = 1.50f;
			float shiftY = 1.50f;

			for (int i = 0; i < rows; i++){
				yPos -= shiftY;
				xPos = -3.75f;
				for (int k = 0; k < cols; k++){
					xPos += shiftX;
					Instantiate (goodButton, new Vector3(xPos, yPos, 0), goodButton.transform.rotation);
				}
			}
        }else if (gameMode == 2){ //Only make good buttons
			for (int i = 0; i < numGoodButtons + 1; i++){
				Instantiate (goodButton, new Vector3(0, 0, 0), goodButton.transform.rotation);
			}
		}
	}

	//Called to remove all the button objects
	void RemoveButtons(){
		GameObject[] buttons;
		buttons = GameObject.FindGameObjectsWithTag ("button");
		if (buttons.Length > 0){
			for (int i = 0; i < buttons.Length; i++){
				Destroy (buttons[i]);
			}
		}
	}

	//Starts a new game taking in an integer for which game mode
	//0 = Standard
	//1 = 10 second tap test
	//2 = 10 second Accuracy test
	public void NewGame(int mode){
		//Set the new game mode
		gameMode = mode;

		newHigh = false;
		timesUp = false;
		missed = false;
		hitButton = false;
		showMenu = false;
		showReplayScreen = false;
		noMiss = false;
		score = 0;
		streak = 0;

		gameObject.GetComponent<UIManagement>().NewMenu ();

		ButtonGood.buttonNum = 0; //Reset the buttonNums

		//displayButton.GetComponent<SpriteRenderer>().enabled = false;
		if (mode == 0){ //Challenge
			BeginCountDown (startTime);
		}else if (mode == 1){ //Speed Test
			BeginCountDown (15.0f);
		}else if (mode == 2){ //Accuracy Test
			BeginCountDown (15.0f);
		}
		GenerateButtons(mode);
    }

	//Method that starts the countdown time and takes in how much time
	void BeginCountDown(float time){
		gameOver = false;
		gameObject.GetComponent<AudioManager>().PlayCountDown ();
		timer = time;
		guiCountDown = 3;
		InvokeRepeating ("TapStartCountDown", .35f, .35f);
	}

	void ShowText(){
		if (showTxt)
			showTxt = false;
		else
			showTxt = true;
	}

    void TapStartCountDown(){
		guiCountDown--;
		if (guiCountDown == 0){
			if (gameMode != 1) //If accuracy or challenge then make it so you can't miss
				noMiss = true;
			InvokeRepeating ("CountDown", .01f, .01f);
		}else if (guiCountDown <= -1){
			CancelInvoke ("TapStartCountDown");
		}
	}

	//Method used to increase the score in the game
	public void IncreaseScore(){
		//if the menu isn't up then increase the score
		if (!showMenu && !showReplayScreen){
			//Increase score by 1
			score++;
			//Get streak bonuses only in challenge mode
			if (gameMode == 0){
				CheckNewStreak ();
				streak++;
				//If the streak hit a specified amount then increase the timer if it is not the speed test game mode
				if (streak % addMoreTimeGoal == 0){ //Only increase the timer if it is the challenge mode
					gameObject.GetComponent<UIManagement>().ShowExtraTime ();
					timer += additionalTime;
					gameObject.GetComponent<AudioManager>().PlayExtraTime ();
				}
			}
		}
	}

    //Called when the game ends
	public void GameOver(int mode){
		gameOver = true;
		CancelInvoke ("CountDown");
		RemoveButtons ();
		ShowReplayScreen ();
		ButtonBad.buttonNum = 0;
		ButtonGood.buttonNum = 0;
		UM_GameServiceManager.instance.SubmitScore("Total_Cracked", PlayerPrefs.GetInt ("totalBroke"));
		if (mode == 0){ //Challenge
			//Check achievements
			if (score >= 100){
				UM_GameServiceManager.instance.UnlockAchievement("Score_100_on_Challenge_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Score_50_on_Challenge_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Play_Challenge_Mode");
			}else if (score >= 50){
				UM_GameServiceManager.instance.UnlockAchievement("Score_50_on_Challenge_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Play_Challenge_Mode");
			}else{
				UM_GameServiceManager.instance.UnlockAchievement("Play_Challenge_Mode");
			}

			if (PlayerPrefs.GetInt ("best") < score){
				UM_GameServiceManager.instance.SubmitScore("High_Score_Challenge", score);
				PlayerPrefs.SetInt ("best", score);
				gameObject.GetComponent<AudioManager>().PlayNewHigh();
				gameObject.GetComponent<UIManagement>().UpdateHighScores();
				newHigh = true;
			}else if (timesUp){
				gameObject.GetComponent<AudioManager>().PlayTimesUp();
			}
		}else if (mode == 1){ //Speed
			//Check achievements
			if (score >= 150){
				UM_GameServiceManager.instance.UnlockAchievement("Score_150_on_Lightning_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Score_80_on_Lightning_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Play_Lightning_Mode");
			}else if (score >= 80){
				UM_GameServiceManager.instance.UnlockAchievement("Score_80_on_Lightning_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Play_Lightning_Mode");
			}else{
				UM_GameServiceManager.instance.UnlockAchievement("Play_Lightning_Mode");
			}

			if (PlayerPrefs.GetInt ("speedBest") < score){
				UM_GameServiceManager.instance.SubmitScore("High_Score_Lightning", score);
				PlayerPrefs.SetInt ("speedBest", score);
				gameObject.GetComponent<AudioManager>().PlayNewHigh();
				gameObject.GetComponent<UIManagement>().UpdateHighScores();
				newHigh = true;
			}else if (timesUp){
				gameObject.GetComponent<AudioManager>().PlayTimesUp();
			}
		}else if (mode == 2){ //Accuracy
			//Check achievements
			if (score >= 40){
				UM_GameServiceManager.instance.UnlockAchievement("Score_40_on_Accuracy_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Score_20_on_Accuracy_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Play_Accuracy_Mode");
			}else if (score >= 20){
				UM_GameServiceManager.instance.UnlockAchievement("Score_20_on_Accuracy_Mode");
				UM_GameServiceManager.instance.UnlockAchievement("Play_Accuracy_Mode");
			}else{
				UM_GameServiceManager.instance.UnlockAchievement("Play_Accuracy_Mode");
			}

			if (PlayerPrefs.GetInt ("accuracyBest") < score){
				UM_GameServiceManager.instance.SubmitScore("High_Score_Accuracy", score);
				PlayerPrefs.SetInt ("accuracyBest", score);
				gameObject.GetComponent<AudioManager>().PlayNewHigh();
				gameObject.GetComponent<UIManagement>().UpdateHighScores();
				newHigh = true;
			}else if (timesUp){
				gameObject.GetComponent<AudioManager>().PlayTimesUp();
			}
		}
    }

	//Replay the mode
	public void Replay(){
		gameObject.GetComponent<UIManagement>().replayMenu.SetActive (false);
		NewGame(gameMode);
	}

	//Go home
	public void GoToMenu(){
		//Application.LoadLevel ("game");
		ShowMenu ();
	}

	//GUI creation
	void OnGUI () {
		//set up scaling
		float rx = Screen.width / native_width;
		float ry = Screen.height / native_height;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));
		
		float xPos = 5;
		float yPos = 5;

		//GUI.Label (new Rect (xPos, yPos, width, height), "Total Broke: " + PlayerPrefs.GetInt ("totalBroke"));
		//GUI.Label (new Rect (xPos, yPos + shiftY, width, height), "Best Streak: " + PlayerPrefs.GetInt ("bestStreak"));
		xPos = 325;
		yPos = 475;

		GUI.skin = countDownFont;

		if (guiCountDown > 0){
			if (guiCountDown > 1)
				GUI.Label (new Rect (xPos, yPos, 1000, 1000), guiCountDown.ToString());
			else
				GUI.Label (new Rect (xPos + 25, yPos, 1000, 1000), guiCountDown.ToString());
		}else if (guiCountDown == 0){
			GUI.Label (new Rect (xPos - 330, yPos, 1000, 1000), "Crack!");
		}
		
	}

	//Methods used to navigate through GUI
	void ShowMenu(){
		showMenu = true;
		missed = false;
		timesUp = false;
		hitButton = false;
		newHigh = false;
		gameObject.GetComponent<UIManagement>().NewMenu ();
	}

	void ShowReplayScreen(){
		showReplayScreen = true;
		noMiss = false;
		gameObject.GetComponent<UIManagement>().NewMenu ();

	}
}
