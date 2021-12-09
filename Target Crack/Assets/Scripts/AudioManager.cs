using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public Sprite volumeOn;
	public Sprite volumeOff;

	public GameObject volume;
	GameObject bgm;

	public AudioClip menuSelect;
	public AudioClip extraTime;
	public AudioClip crackTarget;
	public AudioClip hitBadTarget;
	public AudioClip highScore;
	public AudioClip timesUp;
	public AudioClip missedStreak;
	public AudioClip countDown;
	//public AudioClip bgm; //Background Music

	void Awake(){
		bgm = GameObject.Find ("BGM");
	}

	public void VolumeSwap(){
		//Turn the volume off if it is on
		if (GetComponent<AudioSource>().volume >= 0.01f){
			volume.GetComponent<Image>().sprite = volumeOff;
			GetComponent<AudioSource>().volume = 0.0f;
			bgm.GetComponent<AudioSource>().volume = 0.0f;
		}else{
			volume.GetComponent<Image>().sprite = volumeOn;
			GetComponent<AudioSource>().volume = 1.0f;
			bgm.GetComponent<AudioSource>().volume = .25f;
		}
	}

	public void PlayCountDown() {
		GetComponent<AudioSource>().PlayOneShot (countDown);
	}

	public void PlayMenuSelect(){
		GetComponent<AudioSource>().PlayOneShot (menuSelect);
	}

	public void PlayBreak(){
		GetComponent<AudioSource>().PlayOneShot (crackTarget);
	}

	public void PlayBadTarget(){
		GetComponent<AudioSource>().PlayOneShot (hitBadTarget);
	}

	public void PlayExtraTime(){
		if (!gameObject.GetComponent<GameManagement>().gameOver)
			GetComponent<AudioSource>().PlayOneShot (extraTime);
	}

	public void PlayNewHigh(){
		GetComponent<AudioSource>().PlayOneShot (highScore);
	}

	public void PlayTimesUp(){
		GetComponent<AudioSource>().PlayOneShot (timesUp);
	}

	public void PlayMissed(){
		GetComponent<AudioSource>().PlayOneShot (missedStreak);
	}
}
