using NaNiT.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaNiT.Functions;

namespace NaNiT.Commands
{
    abstract class CommandBase
    {
        public string name { get; set; }
        public string HelpText = "No help available.";
        public string[] aliases { get; set; }
        public List<string> Permission;

        public UserActiveInfo player;
        public ClientThread client;

        public abstract bool doProcess(string[] args);

        

        public void showHelpText()
        {
            this.client.sendCommandMessage("/" + this.name + this.HelpText);
        }
    }
}
