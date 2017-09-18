using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewMenuController : MonoBehaviour {

	public Button WhiteUser;
	public Button BlackUser;
	public Button WhiteCPU;
	public Button BlackCPU;
	public Slider Depth;
	public Slider Time;

	public Text DepthText;
	public Text TimeText;

	private string white = "Player";
	private string black = "Player";


	public void Start(){
		white = PlayerPrefs.GetString("White");
		black = PlayerPrefs.GetString("Black");

		Depth.value = (PlayerPrefs.GetInt("Depth")/2)-1;
		Time.value = (System.Array.IndexOf (new int[] { 1, 2, 3, 4, 5, 10, 15, 20,30 }, PlayerPrefs.GetInt ("Time")));
		DepthText.text = "CPU Depth: " + ((Depth.value + 1) * 2).ToString ();
		TimeText.text = "CPU Time: " + (new int[] { 1, 2, 3, 4, 5, 10, 15,20, 30 }[(int)Time.value]).ToString () + "s";

		if (white != "Player") {
			WhiteUser.gameObject.SetActive (false);
			WhiteCPU.gameObject.SetActive (true);
		} else {
			WhiteUser.gameObject.SetActive (true);
			WhiteCPU.gameObject.SetActive (false);
		}
		if (black != "Player") {
			BlackUser.gameObject.SetActive (false);
			BlackCPU.gameObject.SetActive (true);
		} else {
			BlackUser.gameObject.SetActive (true);
			BlackCPU.gameObject.SetActive (false);
		}
		if (white != "Player" || black != "Player") {
			Depth.gameObject.SetActive (true);
			Time.gameObject.SetActive (true); 
			DepthText.transform.parent.gameObject.SetActive (true); 
			TimeText.transform.parent.gameObject.SetActive (true); 
		} else {
			Depth.gameObject.SetActive (false);
			Time.gameObject.SetActive (false); 
			DepthText.gameObject.SetActive (false); 
			TimeText.gameObject.SetActive (false); 
		}
	}

	public void WhiteHuman(){
		WhiteUser.gameObject.SetActive (false);
		WhiteCPU.gameObject.SetActive (true);
		white = "Computer";
		Depth.gameObject.SetActive (true);
		Time.gameObject.SetActive (true);
		DepthText.gameObject.SetActive (true); 
		TimeText.gameObject.SetActive (true); 
	}

	public void BlackHuman(){
		BlackUser.gameObject.SetActive (false);
		BlackCPU.gameObject.SetActive (true);
		black = "Computer";
		Depth.gameObject.SetActive (true);
		Time.gameObject.SetActive (true);
		DepthText.gameObject.SetActive (true); 
		TimeText.gameObject.SetActive (true); 
	}

	public void WhiteComp(){
		WhiteUser.gameObject.SetActive (true);
		WhiteCPU.gameObject.SetActive (false);
		white = "Player";
		if (black == "Player") {
			
			Depth.gameObject.SetActive (false);
			Time.gameObject.SetActive (false);
			DepthText.gameObject.SetActive (false); 
			TimeText.gameObject.SetActive (false);  
		}
	}

	public void BlackComp(){
		BlackUser.gameObject.SetActive (true);
		BlackCPU.gameObject.SetActive (false);
		black = "Player";
		if (white == "Player") {

			Depth.gameObject.SetActive (false);
			Time.gameObject.SetActive (false);
			DepthText.gameObject.SetActive (false);  
			TimeText.gameObject.SetActive (false);  
		}
	}
		
	public void ChangeDepth(){
		DepthText.text = "CPU Depth: " + ((Depth.value + 1) * 2).ToString ();
	}


	public void ChangeTime(){
		TimeText.text = "CPU Time: " + (new int[] { 1, 2, 3, 4, 5, 10, 15, 20,30 }[(int)Time.value]).ToString () + "s";
	}
		


	public void LauchGame(){
		PlayerPrefs.SetString("White", white);
		PlayerPrefs.SetString("Black", black);
		PlayerPrefs.SetInt ("Depth", new int[] { 2, 4, 6, 8 } [(int)Depth.value]);
		PlayerPrefs.SetInt ("Time", new int[] { 1, 2, 3, 4,5,10,15,20,30 } [(int)Time.value]);
		PlayerPrefs.SetInt ("WhiteTime", 0);
		PlayerPrefs.SetInt ("BlackTime", 0);
		SceneManager.LoadScene("Main");
	}
}