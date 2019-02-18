using NaNiT.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaNiT.Commands
{
    class Kick : CommandBase
    {
        public Kick(ClientThread client)
        {
            this.name = "kick";
            this.HelpText = " <username>: Kicks the user from the server.";
            this.Permission = new List<string>();
            this.Permission.Add("admin.kick");

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {

            string player = string.Join(" ", args).Trim();

            if (player == null || player.Length < 1) { showHelpText(); return false; }

            if (MainProgram.clients.ContainsKey(player))
            {
                MainProgram.clients[player].kickClient(null);
                return true;
            }
            else
            {
                this.client.sendCommandMessage("Player '" + player + "' not found.");
                return false;
            }
        }
    }
}
