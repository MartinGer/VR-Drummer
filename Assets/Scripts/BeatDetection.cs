using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class BeatDetection : MonoBehaviour {

	private float[] spectrum1;
	private float[] spectrum2;
	private float[] spectrum3;
	private float[] spectrum4;
	private float[] lastSpectrum1;
	private float[] lastSpectrum2;
	private float[] lastSpectrum3;
	private float[] lastSpectrum4;
	ArrayList spectralFlux1 = new ArrayList();
	ArrayList spectralFlux2 = new ArrayList();
	ArrayList spectralFlux3= new ArrayList();
	ArrayList spectralFlux4 = new ArrayList();
	Vector3 old1;
	Vector3 old2;
	Vector3 old3;
	Vector3 old4;
	Vector3 oldVecThreshold1;
	Vector3 oldVecThreshold2;
	Vector3 oldVecThreshold3;
	Vector3 oldVecThreshold4;

    private float multiplier1;
    private float multiplier2;
    private float multiplier3;
    private float multiplier4;

    [SerializeField]
    private GameObject flameRed;
    [SerializeField]
    private GameObject flameBlue;
    [SerializeField]
    private GameObject flameGreen;
    [SerializeField]
    private GameObject flameYellow;

    [SerializeField]
	private GameObject sphere1;
    [SerializeField]
    private GameObject sphere2;
    [SerializeField]
    private GameObject sphere3;
    [SerializeField]
    private GameObject sphere4;

    [SerializeField]
	private Material lineMat;

	[SerializeField]
	private Material averageMat;

	[SerializeField]
	private Camera streamCam;

	private int counter;
	private float intervall;
	private readonly int SIZE = 1024;
	private readonly int THRESHOLD_WINDOW_SIZE = 43;

	private GameObject drums;

    readonly int TIMESPAN = 4;

    Vector3 startPos1;
    Vector3 startPos2;
    Vector3 startPos3;
    Vector3 startPos4;

    Vector3 flameRed1Pos;
    Vector3 flameRed2Pos;
    Vector3 flameBlue1Pos;
    Vector3 flameBlue2Pos;
    Vector3 flameGreen1Pos;
    Vector3 flameGreen2Pos;
    Vector3 flameYellow1Pos;
    Vector3 flameYellow2Pos;
    Vector3 flameRot;

    private bool beat = false;
    private bool prunned = false;

    void Start () {
        counter = 2000; //move with the stream cam further away from the game area
        startPos1 = new Vector3(-399.89f, 3.22f, 160);
        startPos2 = new Vector3(-401.04f, 4.76f, 160);
        startPos3 = new Vector3(-403.85f, 4.63f, 160);
        startPos4 = new Vector3(-403.91f, 3.66f, 160);

        flameRed1Pos = new Vector3(-442f, 13f, 147.9f);
        flameRed2Pos = new Vector3(-357f, 13f, 147.9f);
        flameBlue1Pos = new Vector3(-448.3f, 13f, 141.7f);
        flameBlue2Pos = new Vector3(-351.2f, 13f, 141.7f);
        flameGreen1Pos = new Vector3(-456f, 13f, 137.2f);
        flameGreen2Pos = new Vector3(-344.2f, 13f, 137.2f);
        flameYellow1Pos = new Vector3(-462.8f, 13f, 131.8f);
        flameYellow2Pos = new Vector3(-337f, 13f, 131.8f);

        flameRot = new Vector3(-90f, 0f, 0f);

        old1 = new Vector3 (0, 0, 0);
		old2 = new Vector3 (0, 250, 0);
		old3 = new Vector3 (0, 500, 0);
		old4 = new Vector3 (0, 750, 0);
		oldVecThreshold1 = new Vector3 (0, 0, 0);
		oldVecThreshold2 = new Vector3 (0, 250, 0);
		oldVecThreshold3 = new Vector3 (0, 500, 0);
		oldVecThreshold4 = new Vector3 (0, 750, 0);

        intervall = SIZE / 44100f;

		spectrum1 = new float[SIZE];
		spectrum2 = new float[SIZE];
		spectrum3 = new float[SIZE];
		spectrum4 = new float[SIZE];
		lastSpectrum1 = new float[SIZE];
		lastSpectrum2 = new float[SIZE];
		lastSpectrum3 = new float[SIZE];
		lastSpectrum4 = new float[SIZE];

        multiplier1 = 7f;
        multiplier2 = 7f;
        multiplier3 = 7f;
        multiplier4 = 7f;

        drums = GameObject.Find ("Drums");

		InvokeRepeating ("fillArrays", 0, intervall);
		InvokeRepeating ("detectBeat", 0, intervall);
	}
	
	// Update is called once per frame
	void Update () {
		moveCam ();
		setMultiplier ();

        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (prunned)
            {
                prunned = false;
            }
            else prunned = true; 
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            endGame();
        }
    }

    /// <summary>
    /// change the Multiplier by hand
    /// </summary>
    private void setMultiplier(){
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			multiplier1 += 0.25f;
            multiplier2 += 0.25f;
            multiplier3 += 0.25f;
            multiplier4 += 0.25f;
            print (multiplier1);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			multiplier1 -= 0.25f;
            multiplier2 -= 0.25f;
            multiplier3 -= 0.25f;
            multiplier4 -= 0.25f;
            print (multiplier1);
		}
	}

	/// <summary>
	///	create the different frequency containers
	/// </summary>
	private void fillArrays(){
        float[] spectrum = GetComponent<AudioSource>().GetSpectrumData(SIZE, 0, FFTWindow.Hamming);
        spectrum1 = spectrum.Take(3).ToArray ();
		spectrum2 = spectrum.Skip(3).Take (12).ToArray ();
		spectrum3 = spectrum.Skip(15).Take (15).ToArray ();
		spectrum4 = spectrum.Skip(30).Take (30).ToArray ();
	}

	/// <summary>
	///	react on detected beats
	/// </summary>
	private void detectBeat(){
		float value1 = findPeaks(spectrum1, ref lastSpectrum1, ref spectralFlux1,   0, ref old1, ref oldVecThreshold1, ref multiplier1);
		float value2 = findPeaks(spectrum2, ref lastSpectrum2, ref spectralFlux2, 250, ref old2, ref oldVecThreshold2, ref multiplier2);
		float value3 = findPeaks(spectrum3, ref lastSpectrum3, ref spectralFlux3, 500, ref old3, ref oldVecThreshold3, ref multiplier3);
		float value4 = findPeaks(spectrum4, ref lastSpectrum4, ref spectralFlux4, 750, ref old4, ref oldVecThreshold4, ref multiplier4);
		counter = counter + 3;
    //  print(multiplier1 + " " + multiplier2 + " " + multiplier3 + " " + multiplier4);
       
        if (value1 > value2 && value1 > value3 && value1 > value4) {		//consequently also >0
			if (!beat)
            {
				Invoke ("drum1", TIMESPAN);
				Instantiate (sphere1, startPos1, Quaternion.identity);
                beat = true;
				Invoke ("resetBeat", 0.4f);
				if (value1 > 50) {
					Invoke ("redFlames", TIMESPAN);
				}
			}
		}
		if (value2 > value1 && value2 > value3 && value2 > value4) {
			if (!beat)
            {
				Invoke ("drum2", TIMESPAN);
				Instantiate (sphere2, startPos2, Quaternion.identity);
                beat = true;
				Invoke ("resetBeat", 0.4f);
				if (value2 > 50) {
					Invoke ("blueFlames", TIMESPAN);
				}
			}
		}
		if (value3 > value1 && value3 > value2 && value3 > value4) {
			if (!beat)
            {
				Invoke ("drum3", TIMESPAN);
				Instantiate (sphere3, startPos3, Quaternion.identity);
                beat = true;
				Invoke ("resetBeat", 0.4f);
				if (value3 > 50) {
					Invoke ("greenFlames", TIMESPAN);
					}
				}
			}
		if (value4 > value1 && value4 > value2 && value4 > value3) {
			if (!beat)
            {
				Invoke ("drum4", TIMESPAN);
				Instantiate (sphere4, startPos4, Quaternion.identity);
                beat = true;
				Invoke ("resetBeat", 0.4f);
				if (value4 > 50) {
					Invoke ("yellowFlames", TIMESPAN);
						}
					}
				}
      		}

    private void resetBeat()
    {
        beat = false;
    }

    private void redFlames()
    {
        Instantiate(flameRed, flameRed1Pos, Quaternion.Euler(flameRot));
        Instantiate(flameRed, flameRed2Pos, Quaternion.Euler(flameRot));
    }
    private void blueFlames()
    {
        Instantiate(flameBlue, flameBlue1Pos, Quaternion.Euler(flameRot));
        Instantiate(flameBlue, flameBlue2Pos, Quaternion.Euler(flameRot));
    }
    private void greenFlames()
    {
        Instantiate(flameGreen, flameGreen1Pos, Quaternion.Euler(flameRot));
        Instantiate(flameGreen, flameGreen2Pos, Quaternion.Euler(flameRot));
    }
    private void yellowFlames()
    {
        Instantiate(flameYellow, flameYellow1Pos, Quaternion.Euler(flameRot));
        Instantiate(flameYellow, flameYellow2Pos, Quaternion.Euler(flameRot));
    }

    private void drum1()
    {
        drums.transform.GetChild(0).GetComponent<Drum>().colorDrum();
    }

    private void drum2()
    {
        drums.transform.GetChild(1).GetComponent<Drum>().colorDrum();
    }

    private void drum3()
    {
        drums.transform.GetChild(2).GetComponent<Drum>().colorDrum();
    }

    private void drum4()
    {
        drums.transform.GetChild(3).GetComponent<Drum>().colorDrum();
    }

    /// <summary>
    ///	find high ernergpy peaks in an audiofile with a beat detection algorithm
    /// </summary>
    private float findPeaks(float [] spectrum, ref float[] lastSpectrum, ref ArrayList spectralFlux, int transformation, ref Vector3 old, ref Vector3 oldAverage, ref float multiplier){
		
		float flux = 0;
		for (int i = 0; i < spectrum.Length; i++) {	
			float difference = (spectrum [i] - lastSpectrum [i]);	
			flux += difference < 0 ? 0 : difference;
		}
		spectralFlux.Add (flux);

		float mean = 0;
		int fluxNo = spectralFlux.Count;

		if (fluxNo <= THRESHOLD_WINDOW_SIZE) {
			for (int j = 0; j < fluxNo; j++) {
				mean += (float)spectralFlux [j];
			}
		} else {	
			int start = fluxNo - THRESHOLD_WINDOW_SIZE;
			int end = fluxNo;
			for (int j = start; j < end; j++) {
				mean += (float)spectralFlux [j];
			}
		}

		mean /= THRESHOLD_WINDOW_SIZE;
			
		//Varianz
		float varianz = 0;

		if (fluxNo <= THRESHOLD_WINDOW_SIZE) {
			for (int j = 0; j < fluxNo; j++) {
				varianz += Mathf.Pow ((float)spectralFlux [j] - mean, 2);
			}
		} else {	
			int start = fluxNo - THRESHOLD_WINDOW_SIZE;
			int end = fluxNo;
			for (int j = start; j < end; j++) {
				varianz += Mathf.Pow ((float)spectralFlux [j] - mean, 2);
			}
		}
		varianz /= THRESHOLD_WINDOW_SIZE;
			
		mean = mean * multiplier;
		multiplier = calculateMultiplier(varianz*1000);

        if (prunned)
        {
            float prunnedFlux = flux - mean;
		    prunnedFlux = normalize (prunnedFlux);
            if (prunnedFlux < 0) {
			    prunnedFlux = 0;
		    }

            Vector3 vecPrunnedFlux = new Vector3(counter, prunnedFlux + transformation, 0);
            DrawLine(old, vecPrunnedFlux, lineMat, 5);
            old = vecPrunnedFlux;
            flux = prunnedFlux;
        }
        else
        {
            flux = normalize(flux);
            Vector3 vec = new Vector3(counter, flux + transformation, 0);
            DrawLine(old, vec, lineMat, 5);
            old = vec;

            mean = normalize(mean);
            if (flux < mean)
            {
                flux = 0;
            }

            Vector3 vecThreshold = new Vector3(counter, mean + transformation, 0);
            DrawLine(oldAverage, vecThreshold, averageMat, 5);
            oldAverage = vecThreshold;
        }

		lastSpectrum = spectrum;

		return flux;
	}

	/// <summary>
	///	calculate the sensivity depending on the variation
	/// </summary>
	private float calculateMultiplier(float varianz){
		return (varianz * 0.4110f + 5.626f); 
    }

	/// <summary>
	///	moves the Camera with the counter
	/// </summary>
	private void moveCam(){
		streamCam.transform.position = new Vector3 (counter-400, streamCam.transform.position.y, streamCam.transform.position.z);
	}

	/// <summary>
	///	make the drawn Graph better visible
	/// </summary>
	private float normalize(float flux){
		flux = flux * 300;
		return flux;
	}

	/// <summary>
	///	draw a graph with help of a line renderer
	/// </summary>
	private void DrawLine(Vector3 start, Vector3 end, Material mat, float width, float duration = 30f)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = mat;
		lr.receiveShadows = false;
		lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lr.SetWidth (width, width);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

    /// <summary>
    ///	switch back to the other scene after destroying the canvas
    /// </summary>
    private void endGame()
    {
        GameObject canvas = GameObject.Find("Canvas");
        Destroy(canvas);
        SceneManager.LoadScene("Start");
        AudioSource[] audioSources;
        audioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
       	foreach (AudioSource audioSource in audioSources) {
        			audioSource.Stop ();
        	}
          }
}
