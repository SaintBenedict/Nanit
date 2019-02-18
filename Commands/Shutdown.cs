using NaNiT.Packets;
using NaNiT.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaNiT.Commands
{
    class Shutdown : CommandBase
    {
        public Shutdown(ClientThread client)
        {
            this.name = "shutdown";
            this.HelpText = ": Gracefully closes all connections";
            this.Permission = new List<string>();
            this.Permission.Add("admin.shutdown");

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {

            MainProgram.sendGlobalMessage("^#f75d5d;The server is now going down for a restart... We'll be back shortly.");

            MainProgram.allowNewClients = false;

            foreach (ClientThread client in MainProgram.clients.Values)
            {
                client.sendServerPacket(Packet.ClientDisconnect, new byte[1]); //This causes the server to gracefully save and remove the player, and close its connection, even if the client ignores ServerDisconnect.
                client.sendChatMessage("^#f75d5d;You have been disconnected.");
                client.clientState = ClientState.Disposing;
                client.kickTargetTimestamp = Function.getTimestamp() + 7;
            }

            while (MainProgram.clients.Count > 0)
            {
                // Waiting
            }

            MainProgram.logInfo("All connections closed -- Shutting down gracefully.");

            // Saves all groups, in case they were modified while running
            Permissions.Groups.SaveGroups();

            System.Environment.Exit(0);

            return true;
        }
    }

    class Restart : CommandBase
    {
        public Restart(ClientThread client)
        {
            this.name = "restart";
            this.HelpText = "Initiate a restart of the server, 30 second delay.";
            this.Permission = new List<string>();
            this.Permission.Add("admin.restart");

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {

            if (MainProgram.serverState == ServerState.Restarting)
            {
                MainProgram.sendGlobalMessage("^#f75d5d;The server restart has been aborted by " + this.player.name);

                MainProgram.serverState = ServerState.Running;

                MainProgram.restartTime = 0;
            }
            else
            {
                MainProgram.sendGlobalMessage("^#f75d5d;The server will restart in 30 seconds. We will be back shortly.");

                MainProgram.serverState = ServerState.Restarting;

                MainProgram.restartTime = Function.getTimestamp() + 30;
            }

            return true;
        }
    }
}
