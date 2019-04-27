using UnityEngine;
using System.Collections;

public class Drumming : MonoBehaviour {
	public int drumCounter;
	public int count;

	[SerializeField]
	private TextMesh countText;

	// Use this for initialization
	void Start () {
		count = 0;
		setCountText ();
	}
	
	// Update is called once per frame
	void Update () {
		setCountText();
	}

	private void setCountText(){
		countText.text = "Score: " + count.ToString ();
	}
}
