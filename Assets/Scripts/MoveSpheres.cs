using UnityEngine;
using System.Collections;

public class MoveSpheres : MonoBehaviour {
    readonly int TIMESPAN = 4;
	float t;
	Vector3 endPos;
	Vector3 startPos;

    [SerializeField]
    private float endPosZ;

    // Use this for initialization
    void Start () {
        float posX = transform.position.x;
        float posY = transform.position.y;
        endPos = new Vector3(posX, posY, endPosZ);
		startPos = transform.position;

		GameObject.Destroy (gameObject, TIMESPAN);
	}

	// Update is called once per frame
	void Update () {
		t += Time.deltaTime / TIMESPAN;
		transform.position = Vector3.Lerp (startPos, endPos, t);
	}
}
