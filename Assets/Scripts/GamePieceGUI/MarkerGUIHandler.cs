using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerGUIHandler : MonoBehaviour
{
    public GameObject markerGUIPrefab;

    // by sessionId
    private Dictionary<int, MarkerGUI> _markerGUIList;

    // Use this for initialization
    void Start()
    {
        AtmoTracker.OnDetect += HandleOnDetect;
        AtmoTracker.OnUpdate += HandleOnUpdate;
        AtmoTracker.OnLost += HandleOnLost;

        _markerGUIList = new Dictionary<int, MarkerGUI>();
    }

    private void HandleOnDetect(GamePiece marker)
    {
        // Instantiate markerGUI gameObject
        GameObject newMarkerGUIgameObject = Instantiate(markerGUIPrefab);
        // Get markerGUI component
        MarkerGUI newMarkerGUI = newMarkerGUIgameObject.GetComponent<MarkerGUI>();
        // Init markerGUI 
        newMarkerGUI.Init(marker.uniqueId, marker.worldPosition, marker.angle, marker.side, marker.type);
        // Add markerGUI to the list
        _markerGUIList.Add(marker.sessionId, newMarkerGUI);
        
        Debug.Log("New marker on table: " + marker.type + " " + marker.side + " " + marker.rawPosition);
    }

    private void HandleOnUpdate(GamePiece marker)
    {
        // call update on markerGUI only if there's big enough difference
        _markerGUIList[marker.sessionId].UpdateGUI(marker.worldPosition, marker.angle);
    }

    private void HandleOnLost(GamePiece marker)
    {
        try
        {
            // call remove on markerGUI
            _markerGUIList[marker.sessionId].Remove();
        }
        catch (KeyNotFoundException)
        {
            Debug.LogWarning("Couldn't remove an uninitialised markerGUI.");
        }
        
        // remove markerGUI from Dictionary
        _markerGUIList.Remove(marker.sessionId);
        
        Debug.Log("Lost marker: " + marker.type + " " + marker.side + " " + marker.rawPosition);
    }
}
