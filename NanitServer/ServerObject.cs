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
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public static List<ClientObject> clients = new List<ClientObject>(); // все подключения
        public ClientObject[] ClientsToClose;
        public ClientObject withoutList;

        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, gl_i_servPort);
                tcpListener.Start();
                string message = "Сервер запущен: " + DateTime.Now.ToString();
                gl_b_disconnectInProgress = false;
                gl_i_MessageIn = SFunctions.ChangeMesIn(gl_i_MessageIn, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ServerObject(Listen) " + ex.Message);
                Disconnect();
            }

            while (true)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    // Вот тут кончается то, что относится напрямую к серверной части и начинается генерация клиента
                    // Продолжение серверной части видимо будет уже в Процессе у клиента
                    ClientObject clientObject = new ClientObject(tcpClient, this)
                    {
                        myMessageNotAwait = false,
                        CloseMePliz = false,
                        IsRegister = false,
                        dateOfRegister = null,
                        dateLastSeen = null,
                        IsActive = true,
                        StupidCheck = false
                    };
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
                catch (Exception ex) { MessageBox.Show("ServerObject(Lis_Tcp_cli) " + ex.Message); break; }
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
        protected internal void RemoveConnection(string id, string crypto, NetworkStream endStreamofThis, TcpClient lastCryOfYours)
        {
            try
            {
                //
                foreach (ClientObject clTemp in clients)
                {

                    if (clTemp.Id == id || clTemp.cryptoLogin == crypto)
                    {
                        if (clTemp != null)
                        {
                            withoutList = clTemp;
                        }
                    }
                }
                BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, withoutList, "self");
                gl_i_MessageIn = SFunctions.ChangeMesIn(gl_i_MessageIn, withoutList.userName + " отключился " + DateTime.Now.ToString());
                if (endStreamofThis != null) { endStreamofThis.Close(); endStreamofThis = null; }
                clients.Remove(withoutList);
                if (lastCryOfYours != null) { lastCryOfYours.Close(); lastCryOfYours = null; }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return;
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
                        for (int i = 0; i < clientsTemp.Count; i++)
                        {
                            clients[i].Stream.Write(data, 0, data.Length); //передача данных
                        }
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
            bool CompletDel = false;
            if (!gl_b_disconnectInProgress)
            {
                try
                {
                    gl_b_disconnectInProgress = true;
                    gl_i_MessageIn = SFunctions.ChangeMesIn(gl_i_MessageIn, "Сервер прекратил работу: " + DateTime.Now.ToString());
                    Thread ClosedAll = new Thread(new ThreadStart(ClosedAllClients));
                    if (ClosedAll.Name == null)
                        ClosedAll.Name = "Close all clients";
                    ClosedAll.Start();
                    ClosedAll.Join();
                    if (CompletDel == true)
                    {
                        //ClosedAll.Abort();
                        //ClosedAll = null;
                        tcpListener.Stop(); //остановка сервера
                                            //tcpListener = null;
                                            //FormSOptions.listenThread.Abort();
                                            //FormSOptions.listenThread = null;
                                            //FormSOptions.server = null;
                                            //FormSOptions.server = new ServerObject();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception ex) { MessageBox.Show("ServerObject(DCon_main) " + ex.Message); }

            }

            void ClosedAllClients()
            {
                if (clients != null)
                    while (clients.Count != 0)
                    {
                        try
                        {
                            BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, null, "all");
                            foreach (ClientObject clTemp in clients)
                            {
                                if (clTemp != null)
                                {
                                    gl_i_MessageIn = SFunctions.ChangeMesIn(gl_i_MessageIn, clTemp.userName + " отключился");

                                    clTemp.Stream.Close();
                                    //clTemp.Stream.Dispose();
                                    clTemp.client.Close();
                                    //clTemp.client = null;
                                }
                            }
                            clients.Clear();
                        }
                        catch (Exception ex) { MessageBox.Show("ServerObject(DCon_close) " + ex.Message); }
                    }
                CompletDel = true;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}