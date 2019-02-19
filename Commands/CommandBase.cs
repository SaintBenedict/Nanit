using System.Collections.Generic;

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
        }
    }
}
