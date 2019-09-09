using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using System.Linq;

// Parses an osc bundle, argument is an OSCPacket which is a bundle
public class ReactivisionMessageParser : IMessageParser
{
    private List<int> _prevSessionIdList = new List<int>();
    private int _port = 3333;

    public int Port
    {
        get { return _port; }
    }

    public MessageParserResult ParseOscMessages(List<OSCMessage> oscMessages, Homography homography)
    {
        MessageParserResult result = new MessageParserResult();
        bool thereWasNo2DobjMessage = true;

        // parse messages in bundle
        foreach (OSCMessage msg in oscMessages)
        {
            string messageType = msg.Data[0].ToString();

            if (msg.Address != "/tuio/2Dobj") continue;
            thereWasNo2DobjMessage = false;

            switch (messageType)
            {
                case "alive":
                    msg.Data.RemoveAt(0);
                    result.sessionIdList = msg.Data.OfType<int>().ToList();
                    break;

                case "set":

                    int sessionId = (int)msg.Data[1];
                    int uniqueId = (int)msg.Data[2];
                    Vector2 flattenedPosition = new Vector2((float)msg.Data[3], (float)msg.Data[4]);
                    Vector2 worldPosition = homography.GetWorldPosition(flattenedPosition);
                    float angle = -Mathf.Rad2Deg * (float)msg.Data[5];

                    GamePiece marker = new GamePiece(sessionId, uniqueId, flattenedPosition, worldPosition, angle);

                    // check if its the first or repeated SET of the same sessionId based on that add or edit it in the dictinoray
                    if (result.updatedMarkerList.ContainsKey(sessionId))
                        result.updatedMarkerList[sessionId] = marker;
                    else
                        result.updatedMarkerList.Add(sessionId, marker);

                    break;

                case "fseq":
                    break;
                    
            }
        }

        // Check if there were 2DObject messages and not just unused ones
        // If there were no 2DObject, return prev_sessionIdList as we donát have better info
        if (thereWasNo2DobjMessage)
            result.sessionIdList = _prevSessionIdList;

        _prevSessionIdList = result.sessionIdList;

        return result;
    }
}
