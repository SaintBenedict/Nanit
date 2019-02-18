/using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaNiT.Commands
{
    class Broadcast : CommandBase
    {
        public Broadcast(ClientThread client)
        {
            this.name = "broadcast";
            this.HelpText = " <message>: Sends a server-wide message to all clients.";
            this.Permission = new List<string>();
            this.Permission.Add("admin.broadcast");

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {
            if (!hasPermission()) { permissionError(); return false; }

            string message = string.Join(" ", args).Trim();

            if (message == null || message.Length < 1) { showHelpText(); return false; }

            message = "[Broadcast]: " + message;

            MainProgram.sendGlobalMessage(message);

            return true;
        }
    }
}
