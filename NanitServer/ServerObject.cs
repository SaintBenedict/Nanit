using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NaNiT
{
    public class ServerObject : ServerApplication
    {
        /// <summary>
        /// Сервер для прослушивания
        /// </summary>
        internal TcpListener TcpListener { get; set; }
        /// <summary>
        /// Список подключённых клиентов
        /// </summary>
        internal List<ClientObject> ClientsList { get; set; }
        /// <summary>
        /// Рабочий собирающий информацию о подключающихся пользователях
        /// </summary>
        internal XmlUser MyXmlUser { get; set; }
        public CommandManager TransportMessager { get; set; }


        public ServerObject()
        {
            MainServer = this;
            ClientsList = new List<ClientObject>();
            MyXmlUser = new XmlUser(this);            
        }


        // прослушивание входящих подключений
        public void Starting()
        {
            try
            {
                ServerIsDisconnecting = false;
                TcpListener = new TcpListener(IPAddress.Any, ServerConnectionPort);
                TcpListener.Start();
                TrayNotify.Icon = Resources.net3;
                LogMessageList.Add("Сервер запущен: " + DateTime.Now.ToString());
                NewConnection();
            }
            catch (Exception ex)
            {
                Error.Msg("EP0Li0.1", ex.ToString());
            }
        }

        public void NewConnection()
        {
            while (true)
            {
                try
                {
                    TcpClient thisTcpSocket = TcpListener.AcceptTcpClient();
                    ClientObject newClient = new ClientObject(thisTcpSocket, this);
                    Thread thisClientThread = new Thread(new ThreadStart(newClient.ClientWork));
                    thisClientThread.Start();
                }
                catch (Exception ex)
                {
                    if (!ServerIsDisconnecting)
                        Error.Msg("EP0Li1.1", ex.ToString());
                    break;
                }
            }
        }
        
        protected internal void ClientListing(ClientObject clientObject)
        {
            try
            {
                ClientsList.Add(clientObject);
            }
            catch (Exception ex) { Error.Msg("ED1Cl1.1", ex.ToString()); }
        }
        
        protected internal void ClientDeListing(string id)
        {
            try
            {
                foreach (ClientObject ClientTemp in ClientsList)
                {
                    if (ClientTemp.MyInfo.Guid_id == id)
                    {
                        ClientsList.Remove(ClientTemp);
                        return;
                    }
                }
            }
            catch (Exception ex) { Error.Msg("ED1Cl2.1", ex.ToString()); }
        }
        

        // отключение всех клиентов
        public void Disconnect()
        {
            if (!ServerIsDisconnecting)
            {
                try
                {
                    LogMessageList.Add("Сервер прекратил работу: " + DateTime.Now.ToString());
                    ServerIsDisconnecting = true;
                    TrayNotify.Icon = Resources.net4;
                    TransportMessager.Send("Fu(ck&&DI3-");
                    foreach (ClientObject clTemp in ClientsList)
                    {
                        if (clTemp != null)
                        {
                            LogMessageList.Add(clTemp.MyInfo.UserHostName + " отключился");
                            clTemp.Disconnect();
                        }
                    }
                    TcpListener.Stop();
                    TcpListener = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    ClientsList.Clear();
                }
                catch (Exception ex) { Error.Msg("EP0Dc1.1", ex.ToString()); }
            }
        }
    }
}