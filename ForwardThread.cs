using NaNiT.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NaNiT.Packets;
using NaNiT.Extensions;

namespace NaNiT
{
    class ForwardThread
    {
        BinaryReader mInput;
        BinaryWriter mOutput;
        ClientThread mParent;
        Direction mDirection;
        private string passwordSalt;

        public ForwardThread(ClientThread aParent, BinaryReader aInput, BinaryWriter aOutput, Direction aDirection)
        {
            this.mParent = aParent;
            this.mInput = aInput;
            this.mOutput = aOutput;
            this.mDirection = aDirection;
        }

        public void run()
        {
            try
            {
                for (; ; )
                {
                    if (!this.mParent.connectionAlive)
                    {
                        this.mParent.forceDisconnect("Connection Lost");
                        return;
                    }


                    if (this.mParent.kickTargetTimestamp != 0)
                    {
                        if (this.mParent.kickTargetTimestamp < Function.getTimestamp())
                        {
                            this.mParent.forceDisconnect("Kicked from server");
                            return;
                        }
                        continue;
                    }

                    #region Process Packet
                    //Packet ID and Vaildity Check.
                    uint temp = this.mInput.ReadVarUInt32();
                    if (temp < 1 || temp > 48)
                    {
                        this.mParent.errorDisconnect(mDirection, "Sent invalid packet ID [" + temp + "].");
                        return;
                    }
                    Packet packetID = (Packet)temp;

                    //Packet Size and Compression Check.
                    int packetSize = this.mInput.ReadVarInt32();
                    if (packetSize < 0)
                    {
                        packetSize = -packetSize;
                    }

                    //Create buffer for forwarding
                    byte[] dataBuffer = this.mInput.ReadFully(packetSize);

                    //Do decompression
                    MemoryStream ms = new MemoryStream();
                        ms = new MemoryStream(dataBuffer);

                    //Create packet parser
                    BinaryReader packetData = new BinaryReader(ms);
                    #endregion

                    //Return data for packet processor
                    object returnData = true;

                    if (packetID != Packet.Heartbeat && packetID != Packet.UniverseTimeUpdate)
                    {
                        if (mDirection == Direction.Client)
                        #region Handle Client Packets
                        {
                            #region Protocol State Security
                            ClientState curState = this.mParent.clientState;
                            if (curState != ClientState.Connected)
                            {
                                if (curState == ClientState.PendingConnect && packetID != Packet.ClientConnect)
                                {
                                    this.mParent.forceDisconnect("Violated PendingConnect protocol state with " + packetID);
                                }
                                else if (curState == ClientState.PendingAuthentication && packetID != Packet.HandshakeResponse)
                                {
                                    this.mParent.forceDisconnect("Violated PendingAuthentication protocol state with " + packetID);
                                }
                                else if (curState == ClientState.PendingConnectResponse)
                                {
                                    this.mParent.forceDisconnect("Violated PendingConnectResponse protocol state with " + packetID);
                                }
                            }
                            #endregion

                            if (packetID == Packet.ChatSend)
                            {
                                returnData = new Packet11ChatSend(this.mParent, packetData, this.mDirection).onReceive();
                            }
                            else if (packetID == Packet.ClientConnect)
                            {
                                this.mParent.clientState = ClientState.PendingAuthentication;
                                returnData = new Packet7ClientConnect(this.mParent, packetData, this.mDirection).onReceive();
                                MemoryStream packet = new MemoryStream();
                                BinaryWriter packetWrite = new BinaryWriter(packet);

                                passwordSalt = Function.GenerateSecureSalt();
                                packetWrite.WriteStarString("");
                                packetWrite.WriteStarString(passwordSalt);
                                packetWrite.WriteBE(MainProgram.config.passwordRounds);
                                this.mParent.sendClientPacket(Packet.HandshakeChallenge, packet.ToArray());
                            }
                            else if (packetID == Packet.HandshakeResponse)
                            {
                                string claimResponse = packetData.ReadStarString();
                                string passwordHash = packetData.ReadStarString();

                                string verifyHash = Function.StarHashPassword(MainProgram.config.proxyPass, this.mParent.myInfo.account + passwordSalt, MainProgram.config.passwordRounds);
                                if (passwordHash != verifyHash)
                                {
                                    this.mParent.rejectPreConnected("Your password was incorrect.");
                                }

                                this.mParent.clientState = ClientState.PendingConnectResponse;
                                returnData = false;
                            }
                            else if (packetID == Packet.ModifyTileList || packetID == Packet.DamageTileGroup || packetID == Packet.DamageTile || packetID == Packet.ConnectWire || packetID == Packet.DisconnectAllWires)
                            {
                               
                            }
                        }
                        #endregion
                        else
                        #region Handle Server Packets
                        {
                            if (packetID == Packet.ChatReceive)
                            {
                                returnData = new Packet5ChatReceive(this.mParent, packetData, this.mDirection).onReceive();
                            }
                            else if (packetID == Packet.ProtocolVersion)
                            {
                                uint protocolVersion = packetData.ReadUInt32BE();
                                if (protocolVersion != StarryboundServer.ProtocolVersion)
                                {
                                    MemoryStream packet = new MemoryStream();
                                    BinaryWriter packetWrite = new BinaryWriter(packet);
                                    packetWrite.WriteBE(protocolVersion);
                                    this.mParent.sendClientPacket(Packet.ProtocolVersion, packet.ToArray());

                                    this.mParent.rejectPreConnected("Starrybound Server was unable to handle the parent server protocol version.");
                                    returnData = false;
                                }
                            }
                            else if (packetID == Packet.HandshakeChallenge)
                            {
                                string claimMessage = packetData.ReadString();
                                string passwordSalt = packetData.ReadStarString();
                                int passwordRounds = packetData.ReadInt32BE();

                                MemoryStream packet = new MemoryStream();
                                BinaryWriter packetWrite = new BinaryWriter(packet);
                                string passwordHash = Function.StarHashPassword(MainProgram.config.serverPass, MainProgram.config.serverAccount + passwordSalt, passwordRounds);
                                packetWrite.WriteStarString("");
                                packetWrite.WriteStarString(passwordHash);
                                this.mParent.sendServerPacket(Packet.HandshakeResponse, packet.ToArray());

                                returnData = false;
                            }
                            else if (packetID == Packet.ConnectResponse)
                            {
                                while (this.mParent.clientState != ClientState.PendingConnectResponse) { } //TODO: needs timeout
                                returnData = new Packet2ConnectResponse(this.mParent, packetData, this.mDirection).onReceive();
                            }
                            else if (packetID == Packet.WorldStart)
                            {
                                byte[] planet = packetData.ReadStarByteArray();
                                byte[] worldStructure = packetData.ReadStarByteArray();
                                byte[] sky = packetData.ReadStarByteArray();
                                byte[] serverWeather = packetData.ReadStarByteArray();
                                float spawnX = packetData.ReadSingleBE();
                                float spawnY = packetData.ReadSingleBE();
                                uint clientID = packetData.ReadUInt32BE();
                                bool bool1 = packetData.ReadBoolean();
                            }
                            else if (packetID == Packet.WorldStop)
                            {
                                string status = packetData.ReadStarString();
                            }
                            else if (packetID == Packet.GiveItem)
                            {
                                string name = packetData.ReadStarString();
                                uint count = packetData.ReadVarUInt32();
                                List<object> itemDesc = packetData.ReadStarVariant();
                            }
                        }
                        #endregion
                    }

                    //Check return data
                    if (returnData is bool)
                    {
                        if ((bool)returnData == false) continue;
                    }
                    else if (returnData is int)
                    {
                        if ((int)returnData == -1)
                        {
                            this.mParent.forceDisconnect("Command processor requested to drop client");
                        }
                    }

                    #region Forward Packet
                    //Write data to dest
                    this.mOutput.WriteVarUInt32((uint)packetID);
                        this.mOutput.WriteVarInt32(packetSize);
                        this.mOutput.Write(dataBuffer, 0, packetSize);
                    this.mOutput.Flush();
                    #endregion

                    //If disconnect was forwarded to client, lets disconnect.
                    if (packetID == Packet.ServerDisconnect && mDirection == Direction.Server)
                    {
                        this.mParent.forceDisconnect();
                    }
                }
            }
            catch (EndOfStreamException)
            {
                this.mParent.forceDisconnect();
            }
            catch (Exception e)
            {
                this.mParent.errorDisconnect(mDirection, "ForwardThread Exception: " + e.ToString());
            }
        }
    }
}
