/*
  GamePiece.cs - GamePiece class implementation.
  Created by Atmo, July 18, 2018.
*/

using UnityEngine;

public class GamePiece
{
	
	public enum Type
	{
		Dice,
		Token
	}

	/// <summary>Unique id of detection, remains the same for all three events invoked by the same detection.</summary>
	public int uniqueId;

	public int sessionId;
	/// <summary>Id of the up facing side of the game piece</summary>
	public int side;
	/// <summary>raw position of the marker as it comes from the tracking</summary>
	public Vector2 rawPosition; // need to save this for calibration (Homography transformation)
	/// <summary>postion of the marker in Unity world space.</summary>
	public Vector2 worldPosition;
	/// <summary>rotation of the marker in angle.</summary>
	public float angle;
	/// <summary>width of the marker: 0-1</summary>
	public float width;

	public Type type;

	public GamePiece(int sessionId, int uniqueId, Vector2 rawPosition, Vector2 worldPosition, float angle)
	{
		this.sessionId = sessionId;
		this.uniqueId = uniqueId;
		this.rawPosition = rawPosition;
		this.worldPosition = worldPosition;
		this.angle = angle;
	}
    
	public GamePiece(int sessionId, int uniqueId, Vector2 rawPosition, Vector2 worldPosition, float angle, float width, int side, string type)
	{
		this.sessionId = sessionId;
		this.uniqueId = uniqueId;
		this.rawPosition = rawPosition;
		this.worldPosition = worldPosition;
		this.angle = angle;
		this.width = width;
		this.side = side;

		switch (type)
		{
				case "/dice":
					this.type = Type.Dice;
					break;
				case "/token":
					this.type = Type.Token;
					break;
				default:
					Debug.LogWarning("Unknown GamePiece type: " + type);
					break;
		}
	}
}