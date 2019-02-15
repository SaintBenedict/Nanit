using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;

namespace NaNiT
{
    public class ServerObject : IServer
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
        /// Оператор пользовательских параметров
        /// </summary>
        internal XmlUser ServXUsers { get; set; }
        public TcpClient ThisTcpClient { get; set; }

        public ServerObject()
        {
            ClientsList = new List<ClientObject>();
            ServXUsers = new XmlUser();
        }

        public ClientObject Client(TcpClient _tcpclient, ServerObject _server)
        {
            return new ClientObject(_tcpclient, _server)
            {
                myMessageNotAwait = false,
                IsRegister = false,
                DateOfRegistration = null,
                DateLastOnline = null,
                IsActive = true,
                StupidCheck = false
            };
        }

        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                TcpListener = new TcpListener(IPAddress.Any, gl_i_servPort);
                TcpListener.Start();
                gl_b_disconnectInProgress = false;
                gl_sList_Messages.Add("Сервер запущен: " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                if (!gl_b_disconnectInProgress)
                {
                    MessageBox.Show("ServerObject(Listen) " + ex.Message);
                    Disconnect();
                }
                return;
            }

            while (true)
            {
                try
                {

                    TcpClient tcpClient = TcpListener.AcceptTcpClient();
                    // Вот тут кончается то, что относится напрямую к серверной части и начинается генерация клиента
                    // Продолжение серверной части видимо будет уже в Процессе у клиента
                    if (!gl_b_disconnectInProgress)
                    {
                        ClientObject clientObject = Client(tcpClient, this);

                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    if (!gl_b_disconnectInProgress)
                    {
                        MessageBox.Show("ServerObject(Lis_Tcp_cli) " + ex.Message);
                    }
                    break;
                }
            }

        }

        // добавление клиента
        protected internal void AddConnection(ClientObject clientObject)
        {
            try
            {
                ClientsList.Add(clientObject);
            }
            catch (Exception ex) { MessageBox.Show("ServerObject(Add_cli) " + ex.Message); }
        }

        // удаление одного клиента
        protected internal void RemoveConnection(string id)
        {
            try
            {
                foreach (ClientObject ClTemp in ClientsList)
                {
                    if (ClTemp.Guid_id == id)
                    {
                        ClientsList.Remove(ClTemp);
                        return;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("ServerObject(Rem_cli) " + ex.Message); }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, List<ClientObject> clientsTemp, ClientObject clientOne, string whos)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                switch (whos)
                {
                    case "all":
                        if (clientsTemp.Count >= 1)
                        {
                            for (int i = 0; i < clientsTemp.Count; i++)
                            {
                                ClientsList[i].Stream.Write(data, 0, data.Length); //передача данных
                            }
                            break;
                        }
                        else
                            break;
                    case "self":
                        clientOne.Stream.Write(data, 0, data.Length);
                        break;
                }
            }
            catch (Exception ex) { MessageBox.Show("ServerObject(Brodc) " + ex.Message); }
        }

        // отключение всех клиентов
        public void Disconnect()
        {
            if (!gl_b_disconnectInProgress)
            {
                try
                {
                    gl_sList_Messages.Add("Сервер прекратил работу: " + DateTime.Now.ToString());
                    gl_b_disconnectInProgress = true;
                    BroadcastMessage("Fu(ck&&DI3-", ClientsList, null, "all");
                    foreach (ClientObject clTemp in ClientsList)
                    {
                        if (clTemp != null)
                        {
                            gl_sList_Messages.Add(clTemp.UserHostName + " отключился");
                            clTemp.Disconnect();
                        }
                    }
                    TcpListener.Stop();
                    TcpListener = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    ClientsList.Clear();
                }
                catch (Exception ex) { MessageBox.Show("ServerObject(DCon_main) " + ex.Message); }
            }
        }
    }
}