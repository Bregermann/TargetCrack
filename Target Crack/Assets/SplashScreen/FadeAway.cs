using UnityEngine;
using System.Collections;

public class FadeAway : MonoBehaviour {
	Color temp;
	bool fadeOut = false;

	// Use this for initialization
	void Start () {
		temp = gameObject.GetComponent<SpriteRenderer>().color;
	}
	
	void Update(){
		//Slowly fade in the object then fade out the object till you can't see it then switch scenes
		if (!fadeOut){
			temp.a += .018f;
			gameObject.GetComponent<SpriteRenderer>().color = temp;
			if (temp.a >= 1.60f)
				fadeOut = true;
		}else{
			temp.a -= .018f;
			gameObject.GetComponent<SpriteRenderer>().color = temp;
			if (temp.a <= .25f){
				Application.LoadLevel("game");
			}
		}
	}
}
