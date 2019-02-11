using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public static List<ClientObject> clients = new List<ClientObject>(); // все подключения

        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, gl_i_servPort);
                tcpListener.Start();
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

                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    // Вот тут кончается то, что относится напрямую к серверной части и начинается генерация клиента
                    // Продолжение серверной части видимо будет уже в Процессе у клиента
                    if (!gl_b_disconnectInProgress)
                    {
                        ClientObject clientObject = new ClientObject(tcpClient, this)
                        {
                            myMessageNotAwait = false,
                            IsRegister = false,
                            dateOfRegister = null,
                            dateLastSeen = null,
                            IsActive = true,
                            StupidCheck = false
                        };
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
                clients.Add(clientObject);
            }
            catch (Exception ex) { MessageBox.Show("ServerObject(Add_cli) " + ex.Message); }
        }

        // удаление одного клиента
        protected internal void RemoveConnection(string id)
        {
            try
            {
                foreach (ClientObject ClTemp in clients)
                {
                    if (ClTemp.Id == id)
                    {
                        clients.Remove(ClTemp);
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
                                clients[i].Stream.Write(data, 0, data.Length); //передача данных
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
        protected internal void Disconnect()
        {
            if (!gl_b_disconnectInProgress)
            {
                try
                {
                    gl_sList_Messages.Add("Сервер прекратил работу: " + DateTime.Now.ToString());
                    gl_b_disconnectInProgress = true;
                    BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, null, "all");
                    foreach (ClientObject clTemp in clients)
                    {
                        if (clTemp != null)
                        {
                            gl_sList_Messages.Add(clTemp.userName + " отключился");
                            clTemp.Close();
                        }
                    }
                    tcpListener.Stop();
                    tcpListener = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    clients.Clear();
                }
                catch (Exception ex) { MessageBox.Show("ServerObject(DCon_main) " + ex.Message); }
            }
        }
    }
}