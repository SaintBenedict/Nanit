using System;
using System.Collections.Generic;
using System.Text;
using NaNiT.Functions;
using NaNiT.Extensions;
using NaNiT.Packets;
using NaNiT.Permissions;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

namespace NaNiT
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class ClientThread
    {
        public UserActiveInfo myInfo = new UserActiveInfo();
        public ClientState clientState = ClientState.PendingConnect;

        private TcpClient ConnectingSocket;
        private BinaryReader myStreamToRead;
        private BinaryWriter myStreamToWrite;

        private TcpClient sSocket;
        private BinaryReader sIn;
        private BinaryWriter sOut;

        public int kickTargetTimestamp = 0;
        public bool connectionAlive { get { if (this.ConnectingSocket.Connected && this.sSocket.Connected && this.clientState != ClientState.Disposing) return true; else return false; } }

        public ClientThread(TcpClient newConnection)
        {
            this.ConnectingSocket = newConnection;
        }

        public void run()
        {
            try
            {
                this.myStreamToRead = new BinaryReader(this.ConnectingSocket.GetStream());
                this.myStreamToWrite = new BinaryWriter(this.ConnectingSocket.GetStream());

                IPEndPoint ipep = (IPEndPoint)this.ConnectingSocket.Client.RemoteEndPoint;
                IPAddress ipa = ipep.Address;

                this.myInfo.ip = ipep.Address.ToString();

                MainProgram.logInfo("[" + myInfo.client + "] Accepting new connection.");

                sSocket = new TcpClient();
                sSocket.Connect(IPAddress.Loopback, MainProgram.config.serverPort);

                this.sIn = new BinaryReader(this.sSocket.GetStream());
                this.sOut = new BinaryWriter(this.sSocket.GetStream());

                if (!sSocket.Connected)
                {
                    rejectPreConnected("Starrybound server was unable to connect to the parent server.");
                    return;
                }

                // Forwarding for data from SERVER (sIn) to CLIENT (cOut)
                new Thread(new ThreadStart(new ForwardThread(this, this.sIn, this.myStreamToWrite, Direction.Server).run)).Start();

                // Forwarding for data from CLIENT (cIn) to SERVER (sOut)
                new Thread(new ThreadStart(new ForwardThread(this, this.myStreamToRead, this.sOut, Direction.Client).run)).Start();
            }
            catch (Exception e)
            {
                rejectPreConnected("Starrybound server was unable to connect to the parent server.");
                MainProgram.logException("ClientThread Exception: " + e.Message);
            }
        }

        public void sendClientPacket(Packet packetID, byte[] packetData)
        {
            if (this.kickTargetTimestamp != 0) return;
            try
            {
                this.myStreamToWrite.WriteVarUInt32((uint)packetID);
                this.myStreamToWrite.WriteVarInt32((int)packetData.Length);
                this.myStreamToWrite.Write(packetData);
                this.myStreamToWrite.Flush();
            }
            catch (Exception e)
            {
                this.errorDisconnect(Direction.Client, "Failed to send packet: " + e.Message);
            }
        }

        public void sendServerPacket(Packet packetID, byte[] packetData)
        {
            try
            {
                this.sOut.WriteVarUInt32((uint)packetID);
                this.sOut.WriteVarInt32((int)packetData.Length);
                this.sOut.Write(packetData);
                this.sOut.Flush();
            }
            catch (Exception e)
            {
                this.errorDisconnect(Direction.Server, "Failed to send packet: " + e.Message);
            }
        }

        public void sendCommandMessage(string message)
        {
            sendChatMessage(ChatReceiveContext.CommandResult, "", message);
        }

        public void sendChatMessage(string message)
        {
            sendChatMessage("", message);
        }

        public void sendChatMessage(string name, string message)
        {
            sendChatMessage(ChatReceiveContext.Broadcast, "", message);
        }

        public void sendChatMessage(ChatReceiveContext context, string name, string message)
        {
            if (clientState != ClientState.Connected) return;
            Packet11ChatSend packet = new Packet11ChatSend(this, false, Functions.Direction.Client);
            packet.prepare(context, "", 0, name, message);
            packet.onSend();
        }

        public void sendChatMessage(ChatReceiveContext context, string world, uint clientID, string name, string message)
        {
            if (clientState != ClientState.Connected) return;
            Packet11ChatSend packet = new Packet11ChatSend(this, false, Functions.Direction.Client);
            packet.prepare(context, world, clientID, name, message);
            packet.onSend();
        }
        

        public void kickClient(string reason)
        {
            sendServerPacket(Packet.ClientDisconnect, new byte[1]); //This causes the server to gracefully save and remove the player, and close its connection, even if the client ignores ServerDisconnect.
            sendChatMessage("^#f75d5d;You have been kicked from the server by an administrator.");
            MainProgram.sendGlobalMessage("^#f75d5d;" + this.myInfo.name + " has been kicked from the server!");
            kickTargetTimestamp = Function.getTimestamp() + 7;
        }

        private void doDisconnect(bool log)
        {
            if (this.myInfo.name != null)
            {
                Users.SaveUser(this.myInfo);
                if (MainProgram.clients.ContainsKey(this.myInfo.name))
                {
                    MainProgram.clients.Remove(this.myInfo.name);
                    if (this.kickTargetTimestamp == 0) MainProgram.sendGlobalMessage(this.myInfo.name + " has left the server.");
                    if (!log)
                        MainProgram.logInfo("[" + myInfo.client + "] has left the server.");
                }
            }
            if (this.clientState != ClientState.Disposing)
            {
                this.clientState = ClientState.Disposing;
                try
                {
                    this.ConnectingSocket.Close();
                    this.sSocket.Close();
                }
                catch (Exception) { }
            }
        }

        public void rejectPreConnected(string reason)
        {
            Packet2ConnectResponse packet = new Packet2ConnectResponse(this, false, Direction.Client);
            packet.prepare(reason);
            packet.onSend();
            forceDisconnect(reason);
        }

        public void forceDisconnect(string reason)
        {
            if (this.clientState != ClientState.Disposing)
                MainProgram.logWarn("[" + myInfo.client + "] Dropped for " + reason);
            doDisconnect(true);
        }

        public void errorDisconnect(Direction direction, string reason)
        {
            if (this.clientState != ClientState.Disposing)
                MainProgram.logError("[" + myInfo.client + "] Dropped by parent " + direction.ToString() + " for " + reason);
            doDisconnect(true);
        }


        public void forceDisconnect()
        {
            doDisconnect(false);
        }
    }
}
