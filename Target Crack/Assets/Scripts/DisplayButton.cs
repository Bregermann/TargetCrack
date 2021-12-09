using UnityEngine;
using System.Collections;

public class DisplayButton : MonoBehaviour {
	GameObject GameManager;
	GameManagement GameManagement;

	bool hold = true;
	// Use this for initialization
	void Start () {
		GameManager = GameObject.Find ("GameManager");
		GameManagement = GameManager.GetComponent<GameManagement>();
	}
	
	// Update is called once per frame
	void Update () {
		if (hold != GameManagement.gameOver){
			if (GameManagement.gameOver){
				gameObject.GetComponent<Collider>().enabled = true;
				gameObject.GetComponent<SpriteRenderer>().enabled = true;
			}else{
				gameObject.GetComponent<Collider>().enabled = false;
				gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
			hold = GameManagement.gameOver;
		}
	}
}
