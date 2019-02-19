using NaNiT.Functions;
using System.Collections.Generic;

namespace NaNiT.Commands
{
    class Shutdown : CommandBase
    {
        public Shutdown(ClientThread client)
        {
            name = "shutdown";
            HelpText = ": Gracefully closes all connections";
            Permission = new List<string>();
            Permission.Add("admin.shutdown");

            this.client = client;
            player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {
            foreach (ClientThread client in MainProgram.ActiveClients.Values)
            {
                client.SendServerPacket(Packet.ClientDisconnect, new byte[1]); //This causes the server to gracefully save and remove the player, and close its connection, even if the client ignores ServerDisconnect.
                client.statusOfCurrentClient = ClientState.Disposing;
                client.kickTargetTimestamp = Function.getTimestamp() + 7;
            }

            while (MainProgram.ActiveClients.Count > 0)
            {
                // Waiting
            }

            MainProgram.logInfo("All connections closed -- Shutting down gracefully.");
            

            System.Environment.Exit(0);

            return true;
        }
    }

    class Restart : CommandBase
    {
        public Restart(ClientThread client)
        {
            name = "restart";
            HelpText = "Initiate a restart of the server, 30 second delay.";
            Permission = new List<string>();
            Permission.Add("admin.restart");

            this.client = client;
            player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {

            if (MainProgram.CurrentServerStatus == ServerState.Restarting)
            {
                MainProgram.CurrentServerStatus = ServerState.Running;
            }
            else
            {
                MainProgram.CurrentServerStatus = ServerState.Restarting;
            }

            return true;
        }
    }
}
