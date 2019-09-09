using System.Collections.Generic;

public class MessageParserResult 
{
    public List<int> sessionIdList;
    public Dictionary<int, GamePiece> updatedMarkerList;

    public MessageParserResult()
    {
        sessionIdList = new List<int>();
        updatedMarkerList = new Dictionary<int, GamePiece>();
    }
}
