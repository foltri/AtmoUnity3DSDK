using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class AtmoEventHandler 
{
    public enum AtmoEvents
    {
        OnDetect,
        OnUpdate,
        OnLost,
        
        OnDiceDetect,
        OnDiceUpdate,
        OnDiceLost,
        
        OnTokenDetect,
        OnTokenUpdate,
        OnTokenLost
    };

    private List<int> _sessionIdListRef;
    private Dictionary<int, GamePiece> _markerDictRef;
    private Action<AtmoEvents, GamePiece> _triggerAtmoEvent;

    public AtmoEventHandler(List<int> sessionIdList, Dictionary<int, GamePiece> markerDict, Action<AtmoEvents, GamePiece> triggerAtmoEvent)
    {
        _sessionIdListRef = sessionIdList;
        _markerDictRef = markerDict;
        _triggerAtmoEvent = triggerAtmoEvent;
    }

    public void ProcessParsedMessages( MessageParserResult parsedMessages)
    {
        _HandleNewAndEndedSessions(parsedMessages.sessionIdList);
        _HandleMarkerUpdates(parsedMessages.updatedMarkerList);
    }

    // Lost event
    private void _HandleNewAndEndedSessions(List<int> updatedSessionIdList)
    {
        List<int> endedSessions = _sessionIdListRef.Except(updatedSessionIdList).ToList();
        List<int> newSessions = updatedSessionIdList.Except(_sessionIdListRef).ToList();

        foreach (int s_id in endedSessions)
        {
            if (_markerDictRef[s_id] != null)
            {
                // call lost event
                CallLostEvents(_markerDictRef[s_id]);
            }

            _markerDictRef.Remove(s_id);
            _sessionIdListRef.Remove(s_id);
        }

        foreach (int s_id in newSessions)
        {
            _markerDictRef.Add(s_id, null);
            _sessionIdListRef.Add(s_id);
        }
    }

    // Detected and redetected events
    private void _HandleMarkerUpdates(Dictionary<int, GamePiece> updatedMarkerList)
    {
        foreach (GamePiece marker in updatedMarkerList.Values)
        {
            try
            {
                // save old marker to check if its id is null, meaning that it's a new session
                GamePiece oldMarker = _markerDictRef[marker.sessionId];
                _markerDictRef[marker.sessionId] = marker;

                if (oldMarker == null)
                {
                    CallDetectEvents(marker);
                }
                else
                {
                    CallUpdateEvents(marker);
                }
            }
            // This can probably happen when a set message has an id that was not in the alive
            catch(KeyNotFoundException)
            {
                Debug.Log("Couldn't update an uninitialised markerGUI.");
            }
        }
    }

    private void CallDetectEvents(GamePiece gamePiece)
    {
        _triggerAtmoEvent(AtmoEvents.OnDetect, gamePiece);
    }

    private void CallUpdateEvents(GamePiece gamePiece)
    {
        _triggerAtmoEvent(AtmoEvents.OnUpdate, gamePiece);
    }

    private void CallLostEvents(GamePiece gamePiece)
    {
        _triggerAtmoEvent(AtmoEvents.OnLost, gamePiece);
    }
}
