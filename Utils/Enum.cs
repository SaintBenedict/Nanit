using System;
using System.Collections.Generic;
using System.Text;

namespace NaNiT.Utils
{
    public enum Packet
    {
        HelloServer = 1,
        ClientConnect = 2,
        HandshakeChallenge = 3,
        HandshakeResponse = 4,
        ConnectResponse = 5,
        ClientTimeUpdate = 6,
        ClientDisconnect = 7,
        ServerDisconnect = 8,
        SoftwareResponse = 9,
        SoftwareSending = 10,
        ChatSend = 11,
        Heartbeat = 48, //Not Needed
    }

    public enum Direction
    {
        Server = 0,
        Client = 1,
    }

    public enum LogType
    {
        FileOnly = 0,
        Debug = 1,
        Info = 2,
        Error = 3,
        Exception = 4,
        Fatal = 5,
    }

    public enum _ClientState
    {
        PendingConnect = 0,
        PendingAuthentication = 1,
        PendingConnectResponse = 2,
        Connected = 3,
        Disposing = 4,
        Starting = 5,
        Running = 6,
        Aborted = 7,
    }

    public enum ClientState
    {
        PendingConnect = 0,
        PendingAuthentication = 1,
        PendingConnectResponse = 2,
        Connected = 3,
        Disposing = 4,
        Starting = 5,
        Running = 6,
        Aborted = 7,
    }

    public enum ServerState
    {
        Starting = 0,
        Running = 1,
        Restarting = 2,
        Crashed = 3,
        Stoped = 4,
    }
}
