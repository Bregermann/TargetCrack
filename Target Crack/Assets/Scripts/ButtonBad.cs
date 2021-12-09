using UnityEngine;
using System.Collections;

public class ButtonBad : MonoBehaviour {
	public GameObject[] broken;

	GameObject GameManager;
	GameManagement GameManagement;

	float speed = 3.0f;
	float rotateSpeed = 75.0f;

	public static float buttonNum = 0;
	
	float thisButton;

	Vector3 destination;

	float speedMax = 4.10f;
	float speedMin = 2.70f;

	float minSize = .55f;
	float maxSize = .55f;

	Vector3 tempPos;
	// Use this for initialization
	void Start () {
		tempPos = transform.position;

		GameManager = GameObject.Find ("GameManager");
		GameManagement = GameManager.GetComponent<GameManagement>();

		//Only make the buttons move around if the gameMode is not the speed test
		if (GameManagement.gameMode != 1){
			speed = SetSpeed();
			ChangeDestination ();
		}

		buttonNum++;
		thisButton = buttonNum;
		destination.z = thisButton * 1.25f;
		tempPos.z = thisButton * 1.25f; //Divide by 100 to overlap all of them
		transform.position = tempPos;

		if (gameObject.tag != "displayButton"){
			ChangeSize ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime);
		if (transform.position == destination)
			ChangeDestination ();


		//Detect to see if it got tapped
		CheckButtonHit ();

		//Rotate the object to the right
		transform.Rotate (Vector3.back, rotateSpeed * speed * Time.deltaTime);
	}

	//Method used to see if the button got hit
	void CheckButtonHit(){
		//Detect to see if it got tapped
		if (Input.touchCount > 0){ //Make sure there is a tap on the screen
			//Loop through all the touches
			for (int i = 0; i < Input.touchCount; i++){
				//if the touch phase is just now starting then check to see if you hit anything with it
				if (Input.GetTouch (i).phase == TouchPhase.Began){
					//Check to see if this gameObject got hit by this
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

	//When the mouse is clicked on the button or a touch hits the button
	void ButtonGotHit(){
		//Make sure countdown timer isn't running
		if (GameManagement.guiCountDown <= 0){
			GameManagement.GetComponent<AudioManager>().PlayBadTarget ();
			GameObject temp;
			temp = (GameObject)Instantiate (broken[Random.Range (0, broken.Length)], new Vector3(transform.position.x, transform.position.y, transform.position.z + 90.0f), transform.rotation);
			temp.transform.localScale = gameObject.transform.localScale * 1.20f;
			GameManagement.hitButton = true;
			GameManagement.GameOver(0);
		}
	}

	//Changes where the button is moving to and change its speed
	void ChangeDestination(){
		destination = Camera.main.ScreenToWorldPoint (new Vector3(Random.Range (0, Screen.width), Random.Range (100, Screen.height), 0));
		destination.z = thisButton * 1.25f; //Change the z so they overlap eachother properly
		speed = SetSpeed();
	}

	//Method used to randomize the objects size
	void ChangeSize(){
		Vector3 tempSize = transform.localScale;
		tempSize.x = Random.Range (minSize, maxSize);
		tempSize.y = tempSize.x;
		transform.localScale = tempSize;
	}

	//Returns a random float between the speed min and max
	float SetSpeed(){
		return(Random.Range (speedMin, speedMax));
	}
}
