using System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class AtmoTrackerEmulator : MonoBehaviour
{

    public GameObject emulatedDice;
    public GameObject emulatedToken;
    public bool createDice = true;

    private GameObject _canvas;
    private Text _chosenGamePiece;
    
    public static AtmoTrackerEmulator Instance { get; private set; }

    private void Awake()
    {
        // make only one instance of AtmoTracker
        if (Instance == null)
        {                                                                                                                                                                                                    
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        
        // destroy this gameObject if it's not the first instance
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _canvas = transform.GetChild(0).gameObject;
        _chosenGamePiece = _canvas.transform.GetChild(1).gameObject.GetComponent<Text>();

        // hide help
        _canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if left button clicked
        if (Input.GetMouseButtonUp((int) MouseButton.LeftMouse) && Input.GetKey(KeyCode.E))
        {
            if (Camera.main != null)
            {
                Vector2 camWorld = Camera.main.ScreenToWorldPoint((Vector2) Input.mousePosition);
                CreateEmulatedGamePiece(camWorld);
            }
            else
            {
                Debug.LogWarning("Couldn't create EmulatedGamePiece as Main camera is missing.");
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowHelp();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            HideHelp();
        }

        if (Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.D))
        {
            ToggleChoseGamePiece();
        }
        
        if (Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.W))
        {
            RemoveAll();
        }
    }

    private void CreateEmulatedGamePiece(Vector2 position)
    {
        GameObject chosenGamePiecePrefab;
        string chosenGamePieceType;
        if (createDice)
        {
            chosenGamePiecePrefab = emulatedDice;
            chosenGamePieceType = "/dice";
        }
        else
        {
            chosenGamePiecePrefab = emulatedToken;
            chosenGamePieceType = "/token";
        }

        EmulatedGamePiece.Create(chosenGamePiecePrefab, chosenGamePieceType, transform.GetChild(1), position);
    }

    private void ToggleChoseGamePiece()
    {
        createDice = !createDice;
        _chosenGamePiece.text = createDice ? "Chosen: Dice" : "Chosen: Token";
    }

    private void HideHelp()
    {
        _canvas.SetActive(false);
    }

    private void ShowHelp()
    {
        _canvas.SetActive(true);
    }

    private void RemoveAll()
    {
        foreach (Transform gp in transform.GetChild(1).transform)
        {
            gp.GetComponent<EmulatedGamePiece>().Remove();
        } 
    }
}
