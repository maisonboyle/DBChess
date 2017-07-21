﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public Button WhiteUser;
	public Button BlackUser;
	public Text WhiteClock;
	public Text BlackClock;
	public Toggle WhiteToggle;
	public Toggle BlackToggle;
	public Slider WhiteSlider;
	public Slider BlackSlider;

	private string white = "Player";
	private string black = "Player";
	private int whiteTime = 0;
	private int blackTime = 0;


	public void Start(){
		white = PlayerPrefs.GetString("White");
		black = PlayerPrefs.GetString("Black");
		if (white != "Player") {
			WhiteSlider.gameObject.SetActive (false);
			WhiteClock.transform.parent.gameObject.SetActive (false);
		}
		if (black != "Player") {
			BlackSlider.gameObject.SetActive (false);
			BlackClock.transform.parent.gameObject.SetActive (false);
		}
		WhiteUser.GetComponentInChildren<Text> ().text = "White: " + white;
		BlackUser.GetComponentInChildren<Text> ().text = "Black: " + black;
	}

	public void ToggleWhitePlayer(){
		if (white == "Player") {
			white = "Computer";
			WhiteSlider.gameObject.SetActive (false);
			WhiteClock.transform.parent.gameObject.SetActive (false);
		} else {
			white = "Player";
			WhiteClock.transform.parent.gameObject.SetActive (true);
			if (WhiteToggle.isOn) {
				WhiteSlider.gameObject.SetActive (true);
			}
		}
		WhiteUser.GetComponentInChildren<Text> ().text = "White: " + white;
	}

	public void ToggleBlackPlayer(){
		if (black == "Player") {
			black = "Computer";
			BlackSlider.gameObject.SetActive (false);
			BlackClock.transform.parent.gameObject.SetActive (false);
		} else {
			black = "Player";
			BlackClock.transform.parent.gameObject.SetActive (true);
			if (BlackToggle.isOn) {
				BlackSlider.gameObject.SetActive (true);
			}
		}
		BlackUser.GetComponentInChildren<Text> ().text = "Black: " + black;
	}

	public void ToggleWhiteClock(){
		if (WhiteSlider.gameObject.activeSelf) {
			WhiteSlider.gameObject.SetActive (false);
			WhiteClock.text = "Clock: Off";
			whiteTime = 0;
		} else {
			WhiteSlider.gameObject.SetActive (true);
			WhiteClock.text = "Clock: " + WhiteSlider.value + ":00";
			whiteTime = (int) WhiteSlider.value;

		}
	}

	public void ToggleBlackClock(){
		if (BlackSlider.gameObject.activeSelf) {
			BlackSlider.gameObject.SetActive (false);
			BlackClock.text = "Clock: Off";
			blackTime = 0;
		} else {
			BlackSlider.gameObject.SetActive (true);
			BlackClock.text = "Clock: " + BlackSlider.value + ":00";
			blackTime = (int) BlackSlider.value;
		}
	}

	public void WhiteTimeChange(){
		WhiteClock.text = "Clock: " + WhiteSlider.value + ":00";
		whiteTime = (int) WhiteSlider.value;
	}

	public void BlackTimeChange(){
		BlackClock.text = "Clock: " + BlackSlider.value + ":00";
		blackTime = (int) BlackSlider.value;
	}



	public void LauchGame(){
		PlayerPrefs.SetString("White", white);
		PlayerPrefs.SetString("Black", black);
		if (white == "Player") {
			PlayerPrefs.SetInt ("WhiteTime", whiteTime);
		} else {
			PlayerPrefs.SetInt ("WhiteTime", 0);
		}
		if (black == "Player") {
			PlayerPrefs.SetInt ("BlackTime", blackTime);
		} else {
			PlayerPrefs.SetInt ("BlackTime", 0);
		}

		SceneManager.LoadScene("Main");
	}
}
