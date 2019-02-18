namespace NaNiT.Functions
{
    public enum Packet
    {
        ProtocolVersion = 1, //Done
        ConnectResponse = 2, //Done
        ServerDisconnect = 3, //Done
        HandshakeChallenge = 4, //Done
        ChatReceive = 5, //Done
        UniverseTimeUpdate = 6, //Not Needed
        ClientConnect = 7, //Done
        ClientDisconnect = 8, //Done
        HandshakeResponse = 9, //Done
        WarpCommand = 10, //Done
        ChatSend = 11, //Done
        ClientContextUpdate = 12, //In Progress - NetStateDelta
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
        Warn = 3,
        Error = 4,
        Exception = 5,
        Fatal = 6,
    }

    public enum ChatReceiveContext
    {
        Broadcast = 1, //Yellow, Universe
        Channel = 0, //Green, Planet
        Whisper = 2, //Light Pink
        CommandResult = 3, //Grey
        White = 4, //White, Not Offical, just defaults to white if not the above.
        //Anything else is White
    }

    public enum ChatSendContext
    {
        Universe = 0,
        Planet = 1,
    }

    public enum ClientState
    {
        PendingConnect = 0,
        PendingAuthentication = 1,
        PendingConnectResponse = 2,
        Connected = 3,
        Disposing = 4,
    }

    public enum ServerState
    {
        Starting = 0,
        Running = 1,
        Restarting = 2,
        Crashed = 3,
    }
}
