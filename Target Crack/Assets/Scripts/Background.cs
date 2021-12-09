using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {
	float speed = 3.0f;
	float rotateSpeed = 50.0f;

	Vector3 destination;
	
	float speedMax = 3.75f;
	float speedMin = 2.75f;

	int count = 0;
	int changeCount = 1;
	int dir;

	// Use this for initialization
	void Start () {
		ChangeDestination ();
	}

	void Update(){
		transform.position = Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime);
		if (transform.position == destination)
			ChangeDestination ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Changes Color based on spin direction
		ChangeColor (gameObject, gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b);

		ChangeStats ();
		Rotate ();
	}

	//Method used to change the direction speed and color randomly
	void ChangeStats(){
		count++;
		if (count >= changeCount){
			count = 0;
			
			dir = Random.Range (0, 2);
			changeCount = Random.Range (20, 120);
			
			rotateSpeed = Random.Range (50.0f, 150.0f);

			//Changes color randomly
			//ChangeColor (gameObject, Random.Range (0.0f, .85f), Random.Range (0.0f, .85f), Random.Range (0.0f, .85f));
		}
	}

	void Rotate(){
		//Rotate the object to the right
		if (dir == 0){
			transform.Rotate (Vector3.forward, rotateSpeed * speed * Time.deltaTime);
		}else{ //Rotate the object to the left
			transform.Rotate (Vector3.back, rotateSpeed * speed * Time.deltaTime);
		}
	}

	//Method that takes in a gameObject and RGB values and sets the gameObjects color to the RGB values
	void ChangeColor(GameObject temp, float r, float g, float b){
		if (dir == 0){
			//Shift between red and green
			if (g > 0.25f){
				r -= .01f;
				g -= .01f;
				b -= .01f;
			}
		}else{
			//Shift between red and green
			if (r < .75f){
				r += .01f;
				g += .01f;
				b += .01f;
			}
		}
		temp.GetComponent<SpriteRenderer>().color = new Color (r, g, b);
	}

	//Changes where the button is moving to and change its speed;
	void ChangeDestination(){
		destination = Camera.main.ScreenToWorldPoint (new Vector3(Random.Range (-50, Screen.width +50), Random.Range (-50, Screen.height + 50), 0));
		destination.z = 100.0f; //Change the z so they overlap eachother properly
		speed = SetSpeed ();
	}

	//Returns a random float between the speed min and max
	float SetSpeed(){
		return(Random.Range (speedMin, speedMax));
	}
}
