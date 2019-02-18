/* 
 * Starrybound Server
 * Copyright 2013, Avilance Ltd
 * Created by Zidonuke (zidonuke@gmail.com) and Crashdoom (crashdoom@avilance.com)
 * 
 * This file is a part of Starrybound Server.
 * Starrybound Server is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Starrybound Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License along with Starrybound Server. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using NaNiT.Functions;
using NaNiT.Extensions;

namespace NaNiT.Commands
{
    class WarpShip : CommandBase
    {
        public WarpShip(ClientThread client)
        {
            this.name = "warpship";
            this.HelpText = " <name>: Teleports you to another player's ship.";

            this.client = client;
            this.player = client.myInfo;
        }

        public override bool doProcess(string[] args)
        {

            string player = string.Join(" ", args).Trim();

            WorldCoordinate loc = this.player.loc;

            if (player == null || player.Length < 1)
            {
                showHelpText();
                return false;
            }
            else
            {
                if (MainProgram.clients.ContainsKey(player))
                {
                    loc = MainProgram.clients[player].myInfo.loc;
                    this.client.sendCommandMessage("Warping ship to " + player + " [" + loc.ToString() + "]");
                }
                else
                {
                    this.client.sendCommandMessage("Player '" + player + "' not found.");
                    return false;
                }
            }

            MemoryStream packetWarp = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packetWarp);

            packetWrite.WriteBE((uint)WarpType.MoveShip);
            packetWrite.Write(loc);
            packetWrite.WriteStarString("");
            client.sendServerPacket(Packet.WarpCommand, packetWarp.ToArray());
            return true;
        }
    }
}
