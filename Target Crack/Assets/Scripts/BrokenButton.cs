using UnityEngine;
using System.Collections;

public class BrokenButton : MonoBehaviour {
	Color temp;

	// Use this for initialization
	void Start () {
		temp = gameObject.GetComponent<SpriteRenderer>().color;
	}
	
	void Update(){
		//Slowly fade out the object till you can't see it then destroy it
		temp.a -= .01f;
		gameObject.GetComponent<SpriteRenderer>().color = temp;
		if (temp.a <= 0){
			Destroy (gameObject);
		}
	}
}
