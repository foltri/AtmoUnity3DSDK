using UnityEngine;
using UnityOSC;
using System.Collections.Generic;

public class OSCControl
{
    private OSCServer _oscServer;
    private int _port;

    public OSCControl(int port)
    {
        _port = port;

        // init OSC
        OSCHandler.Instance.Init();
        // Initialize OSC servers (listeners)
        _oscServer = OSCHandler.Instance.CreateServer("AtmoUnityServer", _port);
        // Set buffer size (bytes) of the server (default 1024)
        _oscServer.ReceiveBufferSize = 16384;//1024 - good for 10 markers
        // Set the sleeping time of the thread (default 10)
        _oscServer.SleepMilliseconds = 10;

    }

    // Reads all the messages received between the previous update and this one
    public List<OSCMessage> ReceiveParsedMessages()
    {
        List<OSCMessage> newMessages = new List<OSCMessage>();
        
        //todo - only read last message in the queue
                
        // Read received messages
        for (int i = 0; i < OSCHandler.Instance.packets.Count; i++)
        {
            // packets can be either messages or bundles of messages
            if (OSCHandler.Instance.packets[i].IsBundle() && OSCHandler.Instance.packets[i] != null)
            {
                List<OSCMessage> messagesFromBundle = GetMessagesFromBundle(OSCHandler.Instance.packets[i]);
                newMessages.AddRange(messagesFromBundle);
            }

            else
            {
                newMessages.Add(OSCHandler.Instance.packets[i] as OSCMessage);
            }

            // Remove them once they have been read.
            OSCHandler.Instance.packets.Remove(OSCHandler.Instance.packets[i]);
            i--;
        }

        return newMessages;
    }

    private List<OSCMessage> GetMessagesFromBundle(OSCPacket pcktBundle)
    {
        List<OSCMessage> messages = new List<OSCMessage>();

        for (int i = 0; i < pcktBundle.Data.Count; i++)

            messages.Add(pcktBundle.Data[i] as OSCMessage);

        return messages;
    }

}