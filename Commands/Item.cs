using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NaNiT.Extensions;
using NaNiT.Functions;

namespace NaNiT.Commands
{
    class Item : CommandBase
    {
        public Item(ClientThread client)
        {
            this.name = "item";
            this.HelpText = "<item> <amount>; Allows you to give items to yourself.";
            this.aliases = new string[] { "give" };
            this.Permission = new List<string>();
            this.Permission.Add("admin.give");

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {
            if (!hasPermission()) { permissionError(); return false; }

            if (args.Length < 2) { showHelpText(); return false; }

            string item = args[0];
            uint count = Convert.ToUInt32(args[1]) + 1;
            if (String.IsNullOrEmpty(item) || count < 1) { showHelpText(); return false; }

            MemoryStream packet = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packet);

            packetWrite.WriteStarString(item);
            packetWrite.WriteVarUInt32(count);
            packetWrite.Write((byte)0); //0 length Star::Variant
            client.sendClientPacket(Packet.GiveItem, packet.ToArray());
            client.sendCommandMessage("Gave you " + (count - 1) + " " + item);

            return true;
        }
    }
}
