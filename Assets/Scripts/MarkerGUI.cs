using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerGUI : MonoBehaviour {

    private int _uniqueId;
    private Vector2 _worldPosition;
    private float _rotation;
    private EmulatedGamePieceLabel _label;

    private void Awake()
    {
        _label = GetComponent<EmulatedGamePieceLabel>();
    }

    public int UniqueId
    {
        get { return _uniqueId; }
        set
        {
            _uniqueId = value;
            InitBasedOnId();
        }
    }

    public Vector2 WorldPosition
    {
        get
        {
            return _worldPosition;
        }
        set
        {
            _worldPosition = value;
            transform.position = _worldPosition;
        }
    }

    public float Rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            // use an exponentially weighted average to eliminate jumps
//            if (Mathf.Abs(_rotation - value) > 50) _rotation = value;
//            else _rotation = 0.9f * _rotation + 0.1f * value;
           
            _rotation = value;
            transform.eulerAngles = new Vector3(0f, 0f, _rotation);
        }
    }

    public void Init(int u_id, Vector2 position, float rotation, int side, GamePiece.Type type)
	{
        UniqueId = u_id;
        UpdateGUI(position, rotation);
        
        _label.SetId(u_id);
        _label.SetSide(side);
        _label.SetType(type);
	}

    public void UpdateGUI(Vector2 position, float rotation)
	{
        WorldPosition = position;
        Rotation = rotation;
	}

	public void Remove()
	{
        Destroy(gameObject);
	}

    protected virtual void InitBasedOnId()
    {
        gameObject.GetComponent<SpriteRenderer>().color = IdColors.GetColor(_uniqueId);
    }

}
