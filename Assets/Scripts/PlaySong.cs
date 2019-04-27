using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlaySong : MonoBehaviour {
    readonly int TIMESPAN = 4;
    private AudioSource audioSource;
	// Use this for initialization
	void Start () {
        SceneManager.sceneLoaded += setupMusic;
    }
	
	// Update is called once per frame
	void Update () {
       
    }

    /// <summary>
    /// start the algorithm in the main scene by setting up the choosen music
    /// </summary>
    public void setupMusic(Scene scene, LoadSceneMode sceneMode)
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if (GameObject.Find("AudioManager") != null)
            {
                    audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
                    GameObject cameraEyes = GameObject.FindGameObjectWithTag("eyes");
                    cameraEyes.GetComponent<AudioSource>().clip = audioSource.clip;
                    cameraEyes.GetComponent<AudioSource>().Play();
                    Invoke("startSong", TIMESPAN);
            }
        }
    }


    private void startSong(){
		audioSource.Play ();
	}
}
