
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Buttons : MonoBehaviour {

	public Text ButtonText;

	public void setName(string name)
	{
		ButtonText.text = name;
	}

	public void setSong(AudioClip song)
	{
		AudioSource audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.clip = song;
	}
} 