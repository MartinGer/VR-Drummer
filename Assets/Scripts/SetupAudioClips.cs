using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class SetupAudioClips : MonoBehaviour {
	private string path;
	private string type;
	public GameObject ButtonTemplate;
	private static ArrayList clips = new ArrayList();

	// Use this for initialization
	void Start () {
		setupPlaylist ();
	}

	//on button click
	public void FileSelect(){
			LoadSong ();
	}

	void LoadSong(){
		StartCoroutine (LoadSongCoroutine ());
	}

	private IEnumerator LoadSongCoroutine() {
		
		//get info about the files in the music folder
		DirectoryInfo dir = new DirectoryInfo (Application.dataPath + "/music");

		//get oggs
		FileInfo[] info = dir.GetFiles ("*.ogg");
		foreach (FileInfo f in info) {
			string path = f.FullName;
			//get the last part of the path, which is the name of the song. 
			//furthermore we cant use the path directly, because the FileInfo gives us \ in the link, we need /
			path = path.Substring(path.LastIndexOf ("\\") + 1);
			WWW www = new WWW ("file://" + Application.dataPath + "/music/" + path);
			yield return www;
			AudioClip clip = www.GetAudioClip (false, false);
			//we have to create the name for the song ourselves, otherwise it's empty
			clip.name = path;
			addSongToPlaylist (clip);
		}

		//same for wavs, unity doesnt support mp3 on windows
		FileInfo[] info2 = dir.GetFiles ("*.wav");
		foreach (FileInfo f in info2) {
			string path = f.FullName;
			path = path.Substring(path.LastIndexOf ("\\") + 1);
			WWW www = new WWW ("file://" + Application.dataPath + "/music/" + path);
			yield return www;
			AudioClip clip2 = www.GetAudioClip (false, false);
			clip2.name = path;
			addSongToPlaylist (clip2);
		}
	}

	//add a button with the song to the playlist
	public void addSongToPlaylist(AudioClip clip){
		GameObject go = Instantiate(ButtonTemplate) as GameObject;
		go.SetActive(true);
		Buttons button = go.GetComponent<Buttons>();
		button.setName (clip.name);
		button.setSong (clip);
		go.transform.SetParent (ButtonTemplate.transform.parent);
		clips.Add (clip);
	}

	//recreate the playlist after switching back into the menue
	public void setupPlaylist(){
		foreach (AudioClip clip in clips) {
			GameObject go = Instantiate(ButtonTemplate) as GameObject;
			go.SetActive(true);
			Buttons button = go.GetComponent<Buttons>();
			button.setName (clip.name);
			button.setSong (clip);
			go.transform.SetParent (ButtonTemplate.transform.parent);
		}
	}

	//clears the current Playlist
	public void clearPlaylist(){
		clips.Clear();
		//instead of deleting all the buttons seperately, I just reload the scene
		GameObject canvas = GameObject.FindWithTag ("Canvas");
		canvas.GetComponent<Canvas> ().enabled = false;
		Destroy (canvas);
		SceneManager.LoadScene("Start");
	}
}