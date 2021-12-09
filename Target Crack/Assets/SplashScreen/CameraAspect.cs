using UnityEngine;
using System.Collections;

public class CameraAspect : MonoBehaviour {

	void Awake(){
		Camera.main.aspect = 9.0f/16.0f;
	}

}
