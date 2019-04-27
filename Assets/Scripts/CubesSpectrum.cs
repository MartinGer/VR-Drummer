using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesSpectrum : MonoBehaviour {
    private int numberOfCubes;
    private GameObject[] cubes;
    private GameObject[] cubes2;
    // Use this for initialization
    void Start () {

        cubes = GameObject.FindGameObjectsWithTag("cubes");
        cubes2 = GameObject.FindGameObjectsWithTag("cubes2");
        numberOfCubes = cubes.Length;
    }
	
	// Update is called once per frame
	void Update () {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming);
        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 oldScale = cubes[i].transform.localScale;
            oldScale.y = spectrum[i] * 150;
            cubes[i].transform.localScale = oldScale;
        }
        for (int j = 0; j < numberOfCubes; j++)
        {
            Vector3 oldScale = cubes2[numberOfCubes-1-j].transform.localScale;
            oldScale.y = spectrum[j] * 150;
            cubes2[numberOfCubes-1-j].transform.localScale = oldScale;
        }
    }
}
