using System;
using System.Net.Sockets;
using System.Threading;

namespace NaNiT
{
    public class ClientObject
    {
        protected internal NetworkStream MyStream { get; set; }
        public ServerObject FatherServer { get; set; }
        public TcpClient TcpSocket { get; private set; }
        public UserInfo MyInfo { get; set; }
        public CommandManager CaptainTransport { get; set; }
        public bool IsConnected { get; set; }
        internal XmlSoft MyXmlSoft { get; set; }


        public ClientObject(TcpClient tcpClient, ServerObject currentServer)
        {
            TcpSocket = tcpClient;
            FatherServer = currentServer;
            MyInfo = new UserInfo(this);
            CaptainTransport = new CommandManager(this);
            MyInfo.UserIpAdress = Convert.ToString(((System.Net.IPEndPoint)TcpSocket.Client.RemoteEndPoint).Address);
            FatherServer.ClientListing(this);
        }

        public void ClientWork()
        {
            try
            {
                CaptainTransport.WaitCommandInCase = 0;
                MyStream = TcpSocket.GetStream();
                IsConnected = true;
                while (TcpSocket != null && !ServerApplication.ServerIsDisconnecting && IsConnected)
                {
                    string MessageTestNull = CaptainTransport.MessageAccepting();
                    if (MessageTestNull == null)
                    {
                        Disconnect();
                        return;
                    }
                    ServerApplication.ThreadName("Client " + MyInfo.UserHostName, Thread.CurrentThread);
                }
            }
            catch (Exception ex)
            {
                if (!ServerApplication.ServerIsDisconnecting && IsConnected)
                {
                    Error.Msg("EP1Cl1.1", ex.ToString());
                }
            }
            finally
            {
                if (!ServerApplication.ServerIsDisconnecting && IsConnected)
                {
                    Disconnect();
                }
            }
        }

        // закрытие подключения
        public void Disconnect()
        {
            if (IsConnected)
            {
                IsConnected = false;
                try
                {
                    MyInfo.DateLastOnline = DateTime.Now.ToString();

                    if (MyStream != null) { MyStream.Close(); MyStream = null; }
                    if (TcpSocket != null) { TcpSocket.Close(); TcpSocket = null; }
                    if (!ServerApplication.ServerIsDisconnecting && !CaptainTransport.TransportProblem)
                    {
                        ServerApplication.LogMessageList.Add(MyInfo.UserHostName + " отключился [" + MyInfo.DateLastOnline + "]");
                        FatherServer.ClientDeListing(MyInfo.Guid_id);
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    if (!CaptainTransport.TransportProblem)
                        Error.Msg("EP1Cl2.1", ex.ToString());
                }
            }
        }
    }
}