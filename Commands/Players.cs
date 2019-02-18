using NaNiT.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaNiT.Commands
{
    class Players : CommandBase
    {
        public Players(ClientThread client)
        {
            this.name = "players";
            this.HelpText = ": Lists all of the players connected to the server.";
            this.aliases = new string[] {"list","who"};

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {
            string list = "";

            int noOfUsers = MainProgram.clients.Count;
            int i = 0;

            foreach (string user in MainProgram.clients.Keys) {
                list = list + user;

                if (i != noOfUsers - 1) list = list + ", ";

                i++;
            }
            this.client.sendCommandMessage(noOfUsers + " : " + list);
            return true;
        }
    }
}
