using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

// Gets marker positions from MarkerRepository
// Updates _h in Homography

// Press 'c' to start calibration
// Press 'space' for next step

public class Calibration : MonoBehaviour {

    private Vector2 _screenSize = new Vector2(1280, 800);

    [FormerlySerializedAs("_calibrationHotspotPrefab")] public GameObject calibrationHotspotPrefab;
    private Vector2[] _projectedPositions;
    private Vector2[] _camPositions = new Vector2[4];


	// Use this for initialization
	void Start () {

        InitProjectedPositions();

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCalibration();
        }
	}

    private void StartCalibration()
    {
        StartCoroutine(CalibrationCoroutine());
    }

    private IEnumerator CalibrationCoroutine()
    {
        Debug.Log("Calibration started.");

        GameObject hotspot = Instantiate(calibrationHotspotPrefab);

        for (int i = 0; i < 4; i++)
        {
            // place hotspots on the corresponding position
            hotspot.transform.position = _projectedPositions[i];
            hotspot.transform.position = new Vector3(hotspot.transform.position.x, hotspot.transform.position.y, 0f);
            
            bool ready = false;

            while (!ready)
            {
                // wait for space
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (AtmoTracker.MarkerDict.Count == 1)
                    {
                        // use rawPosition here as that is the one coming from the tracker
                        _camPositions[i] = AtmoTracker.MarkerDict.Values.ToList()[0].rawPosition;
                        Debug.Log("Proj: " + _projectedPositions[i] + ", Cam: " + _camPositions[i]);
                        ready = true;
                    }
                    else
                        Debug.LogWarning("Only one marker should be on the table!");
                }

                yield return null;
            }
        }

        AtmoTracker.Instance.Homography.SetHomography(_camPositions, _projectedPositions);

        // remove last hotspot from the table
        Destroy(hotspot);

        Debug.Log("Calibration is over.");
    }

    private void InitProjectedPositions()
    {
        int displacement = 200;

        _projectedPositions = new Vector2[] {
            new Vector2(_screenSize.x/2-displacement, _screenSize.y/2-displacement),
            new Vector2(_screenSize.x/2+displacement, _screenSize.y/2-displacement),
            new Vector2(_screenSize.x/2-displacement, _screenSize.y/2+displacement),
            new Vector2(_screenSize.x/2+displacement, _screenSize.y/2+displacement)

        };

        // transform screen points to world points
        for (int i = 0; i < 4; i++)
        {
            _projectedPositions[i] = Camera.main.ScreenToWorldPoint(_projectedPositions[i]);
        }
    }
}
