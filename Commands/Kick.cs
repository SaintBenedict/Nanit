using System.Collections.Generic;

namespace NaNiT.Commands
{
    class Kick : CommandBase
    {
        public Kick(ClientThread client)
        {
            name = "kick";
            HelpText = " <username>: Kicks the user from the server.";
            Permission = new List<string>();
            Permission.Add("admin.kick");

            this.client = client;
            player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {

            string player = string.Join(" ", args).Trim();

            if (player == null || player.Length < 1) { showHelpText(); return false; }

            if (MainProgram.ActiveClients.ContainsKey(player))
            {
                MainProgram.ActiveClients[player].KickClient(null);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
