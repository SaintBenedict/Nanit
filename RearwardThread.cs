﻿using NaNiT.Packets;
using NaNiT.Utils;
using System;
using System.IO;
using System.Text;
using static NaNiT.Utils.Functions;

namespace NaNiT
{
    class RearwardThread
    {
        BinaryReader _biReaderClient;
        BinaryWriter _biWriterClient;
        Connection _clientSender;
        Direction _messageDirection;

        public RearwardThread(Connection _clientThread, BinaryReader _biReader, BinaryWriter _biWriter, Direction _clientSendDirection)
        {
            _clientSender = _clientThread;
            _biReaderClient = _biReader;
            _biWriterClient = _biWriter;
            _messageDirection = _clientSendDirection;
        }

        public void Run()
        {
            try
            {
                for (; ; )
                {
                    if (!_clientSender.connectionAlive)
                    {
                        _clientSender.ForceDisconnect("Connection Lost");
                        return;
                    }
                    if (_clientSender.kickTargetTimestamp != 0)
                    {
                        if (_clientSender.kickTargetTimestamp < Function.getTimestamp())
                        {
                            _clientSender.ForceDisconnect("Kicked from server");
                            return;
                        }
                        continue;
                    }

                    short temp = _biReaderClient.ReadInt16();                        //Packet ID and Vaildity Check.
                    if (temp < 1 || temp > 48)
                    {
                        _clientSender.ErrorDisconnect(_messageDirection, "Sent invalid packet ID [" + temp + "].");
                        return;
                    }
                    Packet packetID = (Packet)temp;
                    int packetSize = _biReaderClient.ReadInt32();                    //Packet Size and Compression Check.
                    if (packetSize < 0)
                    {
                        packetSize = -packetSize;
                    }
                    byte[] dataBuffer = _biReaderClient.ReadBytes(packetSize);          //Create buffer for forwarding
                    MemoryStream ms = new MemoryStream(dataBuffer);
                    BinaryReader packetData = new BinaryReader(ms);                     //Create packet parser
                    object returnData = true;                                           //Return data for packet processor

                    if (packetID != Packet.Heartbeat)
                    {

                        if (packetID == Packet.HandshakeChallenge)
                        {
                            string claimMessage = packetData.ReadString();
                            int passwordRounds = packetData.ReadInt32();

                            MemoryStream packet = new MemoryStream();
                            BinaryWriter packetWrite = new BinaryWriter(packet);
                            byte[] buffer = Encoding.Unicode.GetBytes("");
                            packetWrite.Write((short)buffer.Length);
                            packetWrite.Write(buffer);
                            _clientSender.SendServerPacket(Packet.HandshakeResponse, packet.ToArray());

                            returnData = false;
                        }
                        else if (packetID == Packet.ConnectResponse)
                        {
                            while (_clientSender.MyStateOnServer != ClientState.PendingConnectResponse) { } //TODO: needs timeout
                            returnData = new Packet5ConnectResponse(_clientSender, packetData, _messageDirection).OnReceive();
                        }

                    }

                    if (returnData is bool)                                                                 //Check return data
                    {
                        if ((bool)returnData == false) continue;
                    }
                    else if (returnData is int)
                    {
                        if ((int)returnData == -1)
                        {
                            _clientSender.ForceDisconnect("Command processor requested to drop client");
                        }
                    }

                    _biWriterClient.Write((short)packetID);                                         //Write data to dest
                    _biWriterClient.Write(packetSize);
                    _biWriterClient.Write(dataBuffer, 0, packetSize);
                    _biWriterClient.Flush();

                    if (packetID == Packet.ServerDisconnect && _messageDirection == Direction.Server)       //If disconnect was forwarded to client, lets disconnect.
                    {
                        _clientSender.forceDisconnect();
                    }
                }
            }
            catch (EndOfStreamException)
            {
                _clientSender.forceDisconnect();
            }
            catch (Exception e)
            {
                _clientSender.ErrorDisconnect(_messageDirection, "ForwardThread Exception: " + e.ToString());
            }
        }
    }
}
