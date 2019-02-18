using NaNiT.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaNiT.Functions;

namespace NaNiT.Commands
{
    class Uptime : CommandBase
    {
        public Uptime(ClientThread client)
        {
            this.name = "uptime";
            this.HelpText = ": Shows how long has past since the server was last restarted.";

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {
            int seconds = Function.getTimestamp() - MainProgram.startTime;

            this.client.sendCommandMessage("I have been online for " + string.Format("{0:0} hour(s) {1:0} minute(s) and {2:0} second(s).", seconds / 3600, (seconds / 60) % 60, seconds % 60));

            return true;
        }
    }
}
