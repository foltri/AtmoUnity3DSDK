using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public interface IMessageParser
{
    int Port { get; }
    MessageParserResult ParseOscMessages(List<OSCMessage> oscMessages, Homography homography);
}
