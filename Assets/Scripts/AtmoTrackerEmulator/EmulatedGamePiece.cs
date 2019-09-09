using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class EmulatedGamePiece : MonoBehaviour
{
    private static int _nextId = 0;
    
    private GamePiece _gamePiece;
    private bool _hidden = false;
    private readonly float _rotationConstant = 360 / 3;
    private EmulatedGamePieceLabel _label;
    private float _rotationStartZ;
    
    public static void Create(GameObject emulatedGamePiecePrefab, string emulatedGamepieceType, Transform parent, Vector2 position)
    {
        GameObject newEmulatedGamePiece = Instantiate(emulatedGamePiecePrefab, parent);
        newEmulatedGamePiece.transform.position = position;
        EmulatedGamePiece newInstance = newEmulatedGamePiece.GetComponent<EmulatedGamePiece>();
        int id = _nextId++;
        
        GamePiece tmp = new GamePiece(id, id, Vector2.one, position, 0, 0, 0, emulatedGamepieceType);
        newInstance._gamePiece = tmp;
        AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnDetect, tmp);
        
        //set label
        newInstance._label.SetId(newInstance._gamePiece.sessionId);
        newInstance._label.SetSide(newInstance._gamePiece.side);
        newInstance._label.SetType(newInstance._gamePiece.type);
    }

    private void Awake()
    {
        _label = GetComponent<EmulatedGamePieceLabel>();
    }

    private void ChooseSide(int newSide)
    {
        
        if (_gamePiece.type == GamePiece.Type.Dice || (_gamePiece.type == GamePiece.Type.Token && newSide < 2))
        {
            AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnLost, _gamePiece);
            _gamePiece.side = newSide;
            AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnDetect, _gamePiece);
            
            _label.SetSide(newSide);
            
            Debug.Log("GamePiece id: " + _gamePiece.uniqueId + " side changed to " + _gamePiece.side);
        }
        else
        {
            Debug.Log("Token has only two sides (0, 1).");
        }
    }
    
    private void ToggleHide()
    {
        if (!_hidden)
        {
            _hidden = true;
            AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnLost, _gamePiece);
            
            // change color of sprite
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 0.3f;
            GetComponent<SpriteRenderer>().color = color;
        }
        else
        {
            _hidden = false;
            AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnDetect, _gamePiece);
            
            // change color of sprite
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 1f;
            GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void Remove()
    {
        AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnLost, _gamePiece);
        Destroy(gameObject);
    }

    private void Move(Vector2 position)
    {
        transform.position = position;
        _gamePiece.worldPosition = position;
        AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnUpdate, _gamePiece);
    }

    private void Rotate(Vector2 position)
    {
        float pointerDistance = Vector2.Distance(_gamePiece.worldPosition, position);
        float rotation = pointerDistance * _rotationConstant;
        transform.eulerAngles = new Vector3(0,0,_rotationStartZ + rotation);

        _gamePiece.angle = rotation;
        AtmoTracker.Instance.TriggerAtmoEvent(AtmoEventHandler.AtmoEvents.OnUpdate, _gamePiece);
    }
    
    private void OnMouseDrag()
    {
        if (Camera.main != null)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint((Vector2) Input.mousePosition);
            if (Input.GetKey(KeyCode.R))
            {
                Rotate(mouseWorld);
            }
            else
            {
                Move(mouseWorld);
            }
        }
        else
        {
            Debug.LogWarning("Couldn't drag EmulatedGamePiece as main camera is missing.");
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            int side;
            bool successfulConversion = int.TryParse(Input.inputString, out side);
            if (successfulConversion) ChooseSide(side);
        }
        
        if (Input.GetMouseButtonUp((int) MouseButton.MiddleMouse))
        {
            ToggleHide();
        }

        if (Input.GetMouseButtonUp((int) MouseButton.RightMouse))
        {
            Remove();
        }
        
        // start rotation from current state (without this it always starts from 0)
        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
        {
            _rotationStartZ = transform.eulerAngles.z;
            Debug.Log(_rotationStartZ);
        }
    }
}

