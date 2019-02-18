using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaNiT.Commands
{
    class AdminChat : CommandBase
    {
        public AdminChat(ClientThread client)
        {
            this.name = "admin";
            this.HelpText = "<message>: Sends a message to all online admins.";
            this.aliases = new string[] {"#<message>"};
            this.Permission = new List<string>();
            this.Permission.Add("chat.admin");
            this.Permission.Add("e:admin.chat");

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {
            if (!hasPermission()) { permissionError(); return false; }

            string message = string.Join(" ", args).Trim();

            if (message == null || message.Length < 1) { showHelpText(); return false; }

            if (this.player.group.hasPermission("admin.chat"))
            {
                message = "^#f75d5d;[ADMIN] " + this.player.name + ": " + message;
            }
            else
            {
                message = "^#ff00c7;Message to admins from " + this.player.name + ": " + message;
            }

            foreach (ClientThread client in MainProgram.clients.Values)
            {
                if (client.myInfo.group.hasPermission("admin.chat") || client == this.client) client.sendChatMessage(message);
            }

            return true;
        }
    }
}
