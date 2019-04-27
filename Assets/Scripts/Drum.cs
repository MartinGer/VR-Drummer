using UnityEngine;
using System.Collections;

public class Drum : MonoBehaviour {
	private bool drumYellow;
	private bool hit;
	private GameObject go;

    [SerializeField]
    private GameObject smoke;
    // Use this for initialization
    void Start () {
		drumYellow = false;
    }
	
	// Update is called once per frame
	void Update () {
	}

    /// <summary>
    /// detect a drumh it
    /// </summary>
    private void OnTriggerEnter(Collider other)  
	{  
		go = other.gameObject;
		if (go != null) {
			int i = (int)go.transform.parent.GetComponent<SteamVR_TrackedObject>().index;
			SteamVR_Controller.Input (i).TriggerHapticPulse(3999);
		}
		if (drumYellow) {
			CancelInvoke ("resetDrum");
			GetComponentInParent<Drumming> ().count++;
			resetDrum ();
        }
	}

	public void colorDrum(){
		if (!drumYellow) {
			drumYellow = true;
			GetComponent<Renderer> ().material.color = Color.yellow;
			Invoke ("resetDrum", 0.8f);
			GetComponentInParent<Drumming> ().drumCounter++;
		}
	}

	private void resetDrum() {
		if (drumYellow) {
			drumYellow = false;
			GetComponent<Renderer> ().material.color = Color.white;
		}
	}
}
