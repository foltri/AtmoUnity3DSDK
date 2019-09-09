/*
  Marker.cs - Marker class implementation.
  Created by Atmo, July 18, 2018.
*/

using UnityEngine;

public class Marker
{
    /// <summary>Id of the marker, same for identical markers.</summary>
    public int sessionId;  
    /// <summary>Unique id of detection, remains the same for all three events invoked by the same detection.</summary>
    public int uniqueId;
    /// <summary>raw position of the marker as it comes from the tracking</summary>
    public Vector2 rawPosition; // need to save this for calibration (Homography transformation)
    /// <summary>postion of the marker in Unity world space.</summary>
    public Vector2 worldPosition;
    /// <summary>rotation of the marker in angle.</summary>
    public float angle;
    /// <summary>width of the marker: 0-1</summary>
    public float width;

    public Marker(int sessionId, int uniqueId, Vector2 rawPosition, Vector2 worldPosition, float angle)
    {
        this.sessionId = sessionId;
        this.uniqueId = uniqueId;
        this.rawPosition = rawPosition;
        this.worldPosition = worldPosition;
        this.angle = angle;
    }
    
    public Marker(int sessionId, int uniqueId, Vector2 rawPosition, Vector2 worldPosition, float angle, float width)
    {
        this.sessionId = sessionId;
        this.uniqueId = uniqueId;
        this.rawPosition = rawPosition;
        this.worldPosition = worldPosition;
        this.angle = angle;
        this.width = width;
    }
    
}
