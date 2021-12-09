using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonGood : MonoBehaviour {
	public GameObject[] explode;

	public GameObject[] broken;

	public static float buttonNum = 0;

	float thisButton;

	GameObject GameManager;
	GameManagement GameManagement;

	float speed = 3.0f;
	float rotateSpeed = 75.0f;

	Vector3 destination;

	float speedMax = 2.75f;
	float speedMin = 3.50f;

	float minSize = .50f;
	float maxSize = .75f;

	Vector3 tempPos;
	// Use this for initialization
	void Start () {
		tempPos = transform.position;

		GameManager = GameObject.Find ("GameManager");
		GameManagement = GameManager.GetComponent<GameManagement>();

		if (GameManagement.gameMode != 1){ //If not speed mode then change the destination
			ChangeDestination ();
		}

		speed = SetSpeed ();

		//Increase the buttonNum and set this button to that button num
		//if (gameObject.tag != "displayButton"){
			buttonNum++;
			thisButton = buttonNum;
			destination.z = thisButton * -1.25f;
			tempPos.z = thisButton * -1.25f;
			transform.position = tempPos;
		//}

		if (/*gameObject.tag != "displayButton" &&*/ GameManagement.gameMode == 2){ //Only change the size on the accuracy mode
			ChangeSize ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManagement.gameMode != 1){ //If its not the speed test then move the objects
			Move();
		}else if (GameManagement.gameOver){ //else if the game is over move the display button
			Move();
		}

		//Detect to see if it got tapped
		CheckButtonHit ();

		//Rotate the object to the right
		transform.Rotate (Vector3.forward, rotateSpeed * speed * Time.deltaTime);
	}

	//Method used to see if the button got hit
	void CheckButtonHit(){
		//Detect to see if it got tapped
		if (Input.touchCount > 0){ //Make sure there is a tap on the screen
			//Loop through all the touches
			for (int i = 0; i < Input.touchCount; i++){
				//if the touch phase is just now starting then check to see if you hit anything with it
				if (Input.GetTouch (i).phase == TouchPhase.Began){
					//Check to see if this gameObject got hit by the touch currently being looped through
					if (GameManagement.CheckHit (gameObject, Input.GetTouch (i).position)){
						ButtonGotHit ();
						break;
					}
				}
			}
		}else if (Input.GetMouseButtonDown (0)){ //Detect to see if it got clicked
			if (GameManagement.CheckHit(gameObject, Input.mousePosition)){
				ButtonGotHit ();
			}
		}
	}

	//Method used to move the object
	void Move(){
		transform.position = Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime);
		if (transform.position == destination)
			ChangeDestination ();
	}

	//Method called when the button is tapped or clicked with the mouse
	void ButtonGotHit(){
		//If the sprite was is enabled then you can hit it assuming the coutdown is done and you are not overtop any GUI (eventsystem) unless the game is afoot!
		if (gameObject.GetComponent<SpriteRenderer>().enabled && GameManagement.guiCountDown <= 0 && (!EventSystem.current.IsPointerOverGameObject() || !GameManagement.gameOver)){
			//Don't destroy the object but make it disappear then re-appear
			StartCoroutine (HideThenShow ());
			//Create an explosion effect
			Explode (Random.Range (2,5));

			GameManagement.GetComponent<AudioManager>().PlayBreak ();

			PlayerPrefs.SetInt ("totalBroke", PlayerPrefs.GetInt ("totalBroke") + 1); //Increase the total broken count
			//Increase the score if it wasn't the display button
			//if (gameObject.tag != "displayButton")
				GameManagement.IncreaseScore ();
			
			//If its not the speed test
			if (GameManagement.gameMode != 1 /*|| gameObject.tag == "displayButton"*/){
				ChangeDestination (); //Change its Destination and speed
			}
			
			if (GameManagement.gameMode == 2 /*&& gameObject.tag != "displayButton"*/){ //If its the accuracy test then change sizes
				ChangeSize ();
			}
		}
	}

	//Method used to randmly creat a number of explosions and then create a broken target
	void Explode(int numExplosions){
		GameObject temp;
		for (int i = 0; i < numExplosions; i++){
			Instantiate (explode[Random.Range (0, explode.Length)], transform.position, Quaternion.identity);
		}
		temp = (GameObject)Instantiate (broken[Random.Range (0, broken.Length)], new Vector3(transform.position.x, transform.position.y, transform.position.z + 90.0f), transform.rotation);
		temp.transform.localScale = gameObject.transform.localScale * 1.20f;
	}

	//Method used to make the object look like it was destroyed
	IEnumerator HideThenShow(){
		yield return 0; //wait for 1 frame

		//Move it out of the way
		tempPos.z = 1000.0f;
		transform.position = tempPos;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;

		yield return new WaitForSeconds(.50f);
		//If its not the speed test randomize its position
		if (GameManagement.gameMode != 1 /*|| gameObject.tag == "displayButton"*/){
			//Randomize its current position
			transform.position = Camera.main.ScreenToWorldPoint (new Vector3(Random.Range (0, Screen.width), Random.Range (100, Screen.height), 0));
		}

		//Bring it back
		tempPos.x = transform.position.x;
		tempPos.y = transform.position.y;
		//if (gameObject.tag != "displayButton"){
			tempPos.z = thisButton * -1.25f;
		//}else{
			//tempPos.z = 100.0f;
		//}
		transform.position = tempPos;

		//Randomize its speed again
		speed = SetSpeed ();

		//Only re-enable the displaybutton if the game is over
		//if ((gameObject.tag != "displayButton" && !GameManagement.gameOver) || (gameObject.tag == "displayButton" && GameManagement.gameOver))
			gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}

	//Method used to randomize the objects size
	void ChangeSize(){
		Vector3 tempSize = transform.localScale;
		tempSize.x = Random.Range (minSize, maxSize);
		tempSize.y = tempSize.x;
		transform.localScale = tempSize;
	}

	//Changes where the button is moving to and change its speed;
	void ChangeDestination(){
		destination = Camera.main.ScreenToWorldPoint (new Vector3(Random.Range (0, Screen.width), Random.Range (100, Screen.height), 0));
		//if (gameObject.tag != "displayButton"){
			destination.z = thisButton * -1.25f; //Change the z so they overlap eachother properly
		//}else{
			//destination.z = 100.0f;
		//}
		speed = SetSpeed ();
	}

	//Returns a random float between the speed min and max
	float SetSpeed(){
		return(Random.Range (speedMin, speedMax));
	}
}
