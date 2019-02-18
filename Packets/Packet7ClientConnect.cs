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

using NaNiT.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NaNiT.Extensions;
using NaNiT.Permissions;

namespace NaNiT.Packets
{
    class Packet7ClientConnect : PacketBase
    {
        public Packet7ClientConnect(ClientThread clientThread, Object stream, Direction direction)
        {
            this.mClient = clientThread;
            this.mStream = stream;
            this.mDirection = direction;
        }

        public override Object onReceive()
        {
            BinaryReader packetData = (BinaryReader)this.mStream;

            byte[] assetDigest = packetData.ReadStarByteArray();

            List<object> claim = packetData.ReadStarVariant();
            byte[] UUID = packetData.ReadStarUUID();
            string name = packetData.ReadStarString();
            string species = packetData.ReadStarString();
            byte[] shipWorld = packetData.ReadStarByteArray();
            string account = packetData.ReadStarString();

            // Identify player to server
            this.mClient.myInfo.uuid = Function.ByteArrayToString(UUID).ToLower();
            this.mClient.myInfo.name = name;
            this.mClient.myInfo.account = account;

            string sAssetDigest = Function.ByteArrayToString(assetDigest);
            MainProgram.logDebug("AssetDigest", "[" + this.mClient.myInfo.client + "] [" + sAssetDigest + "]");
            if(MainProgram.config.useAssetDigest)
            {
                if(sAssetDigest != MainProgram.config.assetDigest)
                {
                    this.mClient.rejectPreConnected("Please reinstall Starbound to connect to this server.");
                    return false;
                }
            }

            User userPData = Users.GetUser(name, this.mClient.myInfo.uuid);
            

            if (MainProgram.clients.ContainsKey(name))
            {
                this.mClient.rejectPreConnected("This username is already in use.");
                return false;
            }
            

            NaNiT.Permissions.Group userGroup;
            try
            {
                UserActiveInfo pData = this.mClient.myInfo;
                
                pData.lastOnline = userPData.lastOnline;
                pData.group = userPData.getGroup();

                if (userPData.name != pData.name)
                {
                    this.mClient.rejectPreConnected("Your character data is corrupt. Unable to connect to server.");
                    return false;
                }
            }
            catch (Exception)
            {
                this.mClient.rejectPreConnected("The server was unable to accept your connection at this time.\nPlease try again later.");
                return false;
            }

            return null;
        }

        public override void onSend()
        {
            //This should never happen! We don't NEED to send it!
        }

        public override int getPacketID()
        {
            return 5;
        }
    }
}
