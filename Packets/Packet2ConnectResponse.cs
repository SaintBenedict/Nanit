﻿/* 
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
using NaNiT.Extensions;

namespace NaNiT.Packets
{
    class Packet2ConnectResponse : PacketBase
    {
        Dictionary<string, object> tmpArray = new Dictionary<string, object>();

        public Packet2ConnectResponse(ClientThread clientThread, Object stream, Direction direction)
        {
            this.mClient = clientThread;
            this.mStream = stream;
            this.mDirection = direction;
        }

        public override object onReceive()
        {
            BinaryReader packetData = (BinaryReader)this.mStream;

            bool success = packetData.ReadBoolean();
            uint clientID = packetData.ReadVarUInt32();
            string rejectReason = packetData.ReadStarString();

            this.mClient.myInfo.id = clientID;
            UserActiveInfo player = this.mClient.myInfo;

            if(!success)
            {
                this.mClient.rejectPreConnected("Rejected by parent server: " + rejectReason);
                return true;
            }

            MainProgram.clients.Add(player.name, this.mClient);
            MainProgram.sendGlobalMessage(player.name + " has joined the server!");
            this.mClient.clientState = ClientState.Connected;
            MainProgram.logInfo("[" + this.mClient.myInfo.client + "][" + this.mClient.myInfo.id + "] joined with UUID " + player.uuid);
            return true;
        }

        public void prepare(string rejectReason)
        {
            tmpArray.Add("rejectReason", rejectReason);
        }

        public override void onSend()
        {
            MemoryStream packet = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packet);

            packetWrite.Write(false);
            packetWrite.WriteVarUInt32(0);
            packetWrite.WriteStarString((string)tmpArray["rejectReason"]);

            this.mClient.sendClientPacket(Packet.ConnectResponse, packet.ToArray());
        }

        public override int getPacketID()
        {
            return 2;
        }
    }
}
