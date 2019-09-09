using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using System.Linq;

public class AtmoTuioMessageParser : IMessageParser {

	private List<int> _prevSessionIdList = new List<int>();
    private int _port = 3333;

    public int Port
    {
        get { return _port; }
    }

    public MessageParserResult ParseOscMessages(List<OSCMessage> oscMessages, Homography homography)
    {
        MessageParserResult result = new MessageParserResult();
        bool noOrUnknownMessage = true;

        // parse messages in bundle
        foreach (OSCMessage msg in oscMessages)
        {
            string messageType = msg.Data[0].ToString();

            if (msg.Address != "/tile")
            {
                int i = 0;
                continue;
            }
            noOrUnknownMessage = false;

            switch (messageType)
            {
                case "alive":
                    msg.Data.RemoveAt(0);
                    result.sessionIdList = msg.Data.OfType<int>().ToList();
                    break;

                case "set":
                    string gamePieceType = msg.Data[1].ToString();
                    
                    int uniqueId = (int)msg.Data[2];
                    // sessionId is the same as uniqueId for now
                    int sessionId = uniqueId;
                    
                    int side = (int)msg.Data[3];
                    
                    // transform raw to world position
                    Vector2 rawPosition = new Vector2((float)msg.Data[4], (float)msg.Data[5]);
                    Vector2 worldPosition = homography.GetWorldPosition(rawPosition);
                    
                    // 0-1
                    float width = (float)msg.Data[6];
                    float angle = (float)msg.Data[7]*360;

                    GamePiece gamePiece = new GamePiece(sessionId, uniqueId, rawPosition, worldPosition, angle, width, side, gamePieceType);

                    // check if it's the first or repeated SET of the same sessionId based on that add or edit it in the dictinoray
                    if (result.updatedMarkerList.ContainsKey(sessionId))
                        result.updatedMarkerList[sessionId] = gamePiece;
                    else
                        result.updatedMarkerList.Add(sessionId, gamePiece);

                    break;

                case "fseq":
                    break;
                    
            }
        }

        if (noOrUnknownMessage)
        {
            result.sessionIdList = _prevSessionIdList;
        }

        // save sessionIdList
        _prevSessionIdList = result.sessionIdList;

        return result;
    }
}