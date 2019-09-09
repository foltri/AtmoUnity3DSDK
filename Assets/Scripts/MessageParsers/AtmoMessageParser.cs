using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;


public class AtmoMessageParser : IMessageParser
{
	private MessageParserResult _result;
	private int _port = 7771;

	public AtmoMessageParser()
	{
		_result = new MessageParserResult();
	}

	public int Port
	{
		get { return _port; }
	}

	public MessageParserResult ParseOscMessages(List<OSCMessage> oscMessages, Homography homography)
	{		
		// Clear updated marker list, containing updates have been processed in prev frame
		_result.updatedMarkerList.Clear();
		
		foreach (OSCMessage msg in oscMessages)
		{
			int eventType = int.Parse(msg.Data[0].ToString());
			int uniqueId = int.Parse(msg.Data[1].ToString());
			int sessionId = int.Parse(msg.Data[2].ToString());
			Vector2 pixelPosition = new Vector2(int.Parse(msg.Data[3].ToString()), int.Parse(msg.Data[4].ToString()));
			Vector2 worldPosition = homography.GetWorldPosition(pixelPosition);

			GamePiece newDice = new GamePiece(sessionId, uniqueId, pixelPosition, worldPosition, 0f);
			
			HandleEvent(eventType, newDice);
		}

		return _result;
	}

	private void HandleEvent(int eventType, GamePiece marker)
	{
		switch (eventType)
		{
			//OnDetected
			case 0:
				_result.updatedMarkerList.Add(marker.sessionId, marker);
				_result.sessionIdList.Add(marker.sessionId);
				break;
			
			//OnRedetected
			case 2:
				_result.updatedMarkerList.Add(marker.sessionId, marker);
				_result.sessionIdList.Add(marker.sessionId);
				break;
			
			//OnLost
			case 1:
				_result.sessionIdList.Remove(marker.sessionId);
				break;
			
			default:
				Debug.LogWarning("Unknown event type: " + eventType);
				break;
				
		}
	}
}
