using System.Collections.Generic;
using UnityEngine;
using System;
using UnityOSC;

public class AtmoTracker : MonoBehaviour
{
    // static marker events easily accessible from everywhere
    public static event Action<GamePiece> OnDetect = delegate { };
    public static event Action<GamePiece> OnUpdate = delegate { };
    public static event Action<GamePiece> OnLost = delegate { };

    public static event Action<GamePiece> OnDiceDetect = delegate { };
    public static event Action<GamePiece> OnDiceUpdate = delegate { };
    public static event Action<GamePiece> OnDiceLost = delegate { };
    
    public static event Action<GamePiece> OnTokenDetect = delegate { };
    public static event Action<GamePiece> OnTokenUpdate = delegate { };
    public static event Action<GamePiece> OnTokenLost = delegate { };
    
    
    private List<int> _sessionIdList;
    private Dictionary<int, GamePiece> _markerDict;
    private OSCControl _oscControl;
    private IMessageParser _messageParser;
    private AtmoEventHandler _atmoEventHandler;
    
    // contains markers by sessionIds
    public static Dictionary<int, GamePiece> MarkerDict
    {
        get
        {
            return Instance._markerDict;
        }
    }
    
    // Homography matrix to sync camera and projector plane
    public Homography Homography { get; private set; }

    // make static Instance so it's accessible easily from everywhere
    public static AtmoTracker Instance { get; private set; }

    private void Awake()
    {
        // make only one instance of AtmoTracker
        if (Instance == null)
        {                                                                                                                                                                                                    
            Instance = this;
            
            DontDestroyOnLoad(gameObject);

            _sessionIdList = new List<int>();
            _markerDict = new Dictionary<int, GamePiece>();

            Homography         = new Homography();
            _messageParser      = new AtmoTuioMessageParser();
            _oscControl         = new OSCControl(_messageParser.Port);
            _atmoEventHandler   = new AtmoEventHandler(_sessionIdList, _markerDict, TriggerAtmoEvent);
        }
        
        // destroy this gameObject if it's not the first instance
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // receives OSCMessages
        List<OSCMessage> oscMessages = _oscControl.ReceiveParsedMessages();
        // 
        MessageParserResult parsedMessages = _messageParser.ParseOscMessages(oscMessages, Homography);
        _atmoEventHandler.ProcessParsedMessages(parsedMessages);
    }

    public void TriggerAtmoEvent(AtmoEventHandler.AtmoEvents atmoEvent, GamePiece gamePiece)
    {
        switch (atmoEvent)
        {
            case AtmoEventHandler.AtmoEvents.OnDetect:
                OnDetect(gamePiece);
                switch (gamePiece.type)
                {
                    case GamePiece.Type.Dice:
                        OnDiceDetect(gamePiece);
                        break;
                    case GamePiece.Type.Token:
                        OnTokenDetect(gamePiece);
                        break;
                    default:
                        Debug.LogWarning("Unknown GamePiece.Type.");
                        break;
                }
                break;
            case AtmoEventHandler.AtmoEvents.OnUpdate:
                OnUpdate(gamePiece);
                switch (gamePiece.type)
                {
                    case GamePiece.Type.Dice:
                        OnDiceUpdate(gamePiece);
                        break;
                    case GamePiece.Type.Token:
                        OnTokenUpdate(gamePiece);
                        break;
                    default:
                        Debug.LogWarning("Unknown GamePiece.Type.");
                        break;
                }
                break;
            case AtmoEventHandler.AtmoEvents.OnLost:
                OnLost(gamePiece);
                switch (gamePiece.type)
                {
                    case GamePiece.Type.Dice:
                        OnDiceLost(gamePiece);
                        break;
                    case GamePiece.Type.Token:
                        OnTokenLost(gamePiece);
                        break;
                    default:
                        Debug.LogWarning("Unknown GamePiece.Type.");
                        break;
                }
                break;
            default:
                Debug.LogWarning(atmoEvent + " event is unknown.");
                break;
        }
    }
}
