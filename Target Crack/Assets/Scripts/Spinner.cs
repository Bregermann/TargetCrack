using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {
	bool spinning = false;
	GameObject GameManager;

	/*Used for detecting swipes
	Vector2 startPos;
	Vector2 direction;
	bool directionChosen = false;*/

	// Update is called once per frame
	/*void Update () {
		//If there is a touch on the screen
		if (Input.touchCount == 1){
			//Get the touch
			Touch touch = Input.GetTouch(0);
			//Switch through the touch phase to detect a swipe and see if the sheep jumped
				switch (touch.phase) {
					//Record initial touch position
					case TouchPhase.Began:
						startPos = touch.position;
						directionChosen = false;
						break;

					//Determine direction by comparing the current touch position with the initial one
					case TouchPhase.Moved:
						direction = touch.position - startPos;
						break;

					//Report that a direction has been chosen when the finger is lifted
					case TouchPhase.Ended:
						directionChosen = true;
						break;
				}

				if (directionChosen) {
					//If the y position moved up then jump the sheep
					if (direction.y > startPos.y){
						transform.Rotate (0, 0, -2.5f);
					}
				}
		}
		if (spinning){
			transform.Rotate (0, 0, -3.0f);
		}
	}*/

	void Start(){
		GameManager = GameObject.FindGameObjectWithTag ("GameManager");
	}

	public IEnumerator Spin(){
		for (int i = 0; i < 20; i++){
			transform.Rotate (0, 0, -3.0f);
			yield return (0);
		}
		spinning = false;
	}

	public void StartSpin(){
		if (!spinning){
			StartCoroutine(Spin());
			spinning = true;
			GameManager.GetComponent<AudioManager>().PlayMenuSelect ();
		}
	}

	/*public void SpinOn(){
		spinning = true;
	}

	public void SpinOff(){
		spinning = false;
	}*/
}
