using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
  
	private void playSong(){
		GameObject canvas = GameObject.FindWithTag ("Canvas");
		canvas.GetComponent<Canvas> ().enabled = false;
		
        AudioSource audioSource = GetComponent<AudioSource>();
        GameObject audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioSource>().clip = audioSource.clip;

        SceneManager.LoadScene("MainScene");
    }
}
