using UnityEngine;
using System.Collections;

public class KeepIngame : MonoBehaviour {

	// Use this for initialization
	void Start () {
			DontDestroyOnLoad(this.gameObject);
	}
}
