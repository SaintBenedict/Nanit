using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NaNiT
{
    public class CommandManager
    {
        public bool TransportProblem { get; set; }
        public bool ActiveTransportingMessages { get; set; }
        public int WaitCommandInCase { get; set; }
        private string SendingTarget { get; set; }
        private ClientObject President { get; set; }
        private ServerObject MyServer { get; set; }

        internal CommandManager(ClientObject president)
        {
            President = president;
            ActiveTransportingMessages = false;
            SendingTarget = "self";
        }
        protected internal CommandManager()
        {
            MyServer = ServerApplication.MainServer;
            SendingTarget = "all";
            ActiveTransportingMessages = false;
        }

        public void Send(string _message)
        {
            if (MyServer.ClientsList.Count >= 1 && SendingTarget == "all")
            {
                for (int i = MyServer.ClientsList.Count; i > 1; i--)
                {
                    MessageSending(MyServer.ClientsList[i], _message);
                }
            }
            else if (SendingTarget == "self")
                MessageSending(President, _message);

            void MessageSending(ClientObject currentSender, string message)
            {
                try
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    currentSender.MyStream.Write(data, 0, data.Length);
                }
                catch (Exception ex) { Error.Msg("EP0Tr2.1", ex.ToString()); }
            }
        }

        public string MessageAccepting()
        {
            if (President.MyStream.CanRead)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        if (ServerApplication.ServerIsDisconnecting || !President.IsConnected)
                        {
                            return null;
                        }
                        ActiveTransportingMessages = true;
                        bytes = President.MyStream.Read(data, 0, data.Length);
                        if (bytes == 0)
                            return null;
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (President.MyStream.DataAvailable && President.IsConnected);
                    CheckCommand(builder.ToString(), President, MyServer, President.MyStream);
                    return builder.ToString();
                }
                catch
                {
                    if (!ServerApplication.ServerIsDisconnecting && President.IsConnected)
                    {
                        President.Disconnect();
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            int SoftwareMassive = 0; //Временная переменная с максимальным индексом софт массива
            string[] tempMass = new string[5];
            int checkMass = 0;
            int checkMuss = 0;

            if (message == @"IamStupid-" || message.Length < 10)
            {
                client.MyInfo.DateLastOnline = DateTime.Now.ToString();
                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " отправил неверные данные и был отключён [" + client.MyInfo.DateLastOnline + "]");
                MyServer.TransportMessager.Send("Fu(ck&&DI3-");
                server.ClientDeListing(client.MyInfo.Guid_id);
                ActiveTransportingMessages = false;
                client.Disconnect();
            }
            else
            {
                string command = message.Substring(0, 10);
                string textCom = message.Substring(10, message.Length - 10);
                switch (WaitCommandInCase)
                {

                    case 0: // Не ожидаем команды
                        switch (command)
                        {
                            default: // Дисконект из-за неверного сообщения
                                client.MyInfo.DateLastOnline = DateTime.Now.ToString();
                                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " отправил неверные данные и был отключён [" + client.MyInfo.DateLastOnline + "]");
                                MyServer.TransportMessager.Send("Fu(ck&&DI3-");
                                server.ClientDeListing(client.MyInfo.Guid_id);
                                ActiveTransportingMessages = false;
                                client.Disconnect();
                                break;

                            case @"h@@lLloui-": // Клиент говорит Хэллоу, подключившись, мы спрашиваем кто он и ждём реги
                                client.MyInfo.UserHostName = textCom;
                                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " --- подключился");
                                // Проверка регистрации
                                client.CaptainTransport.Send(@"@HowdyHu%$-"); // спрашиваем кто он такой блин
                                WaitCommandInCase = 1;
                                ActiveTransportingMessages = true;
                                TransportProblem = false;
                                break;

                            case @"CH@T_AlL_-": // Команда админской рассылки мессейджа
                                SendChatToAll(textCom);
                                WaitCommandInCase = 0;
                                ActiveTransportingMessages = false;
                                TransportProblem = false;
                                break;

                            case @"SenDS0fT?-": // Божечки, да нас массивом софта завалят сейчас
                                WaitCommandInCase = 2;
                                ActiveTransportingMessages = true;
                                TransportProblem = false;
                                checkMuss = 0;
                                checkMass = 0;
                                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " --- Начал передачу софт-массива [" + DateTime.Now.ToString() + "]");
                                SoftwareMassive = Convert.ToInt32(textCom);
                                File.Delete(@"ClientsBase\" + client.MyInfo.CryptedName + @"_Software.xml");
                                client.MyXmlSoft = new XmlSoft(server, client, @"ClientsBase\" + client.MyInfo.CryptedName + @"_Software.xml");
                                client.CaptainTransport.Send(@"OKb@@b1_g0-" + checkMuss + @"%" + checkMass); // говорим, что готовы въёбывать и отправляем по базе нулевой чекмас, намекая, что хотим его
                                break;
                        }
                        break;

                    case 1: // Ожидаем регистрацию
                        switch (command)
                        {
                            default: // Дисконект из-за неверного сообщения
                                client.MyInfo.DateLastOnline = DateTime.Now.ToString();
                                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " --- отправил неверные данные и был отключён [" + client.MyInfo.DateLastOnline + "]");
                                MyServer.TransportMessager.Send("Fu(ck&&DI3-");
                                server.ClientDeListing(client.MyInfo.Guid_id);
                                ActiveTransportingMessages = false;
                                client.Disconnect();
                                break;

                            case @"R3GisSsTr-": // получаем регу и отвечаем, что она успешно прошла
                                client.MyInfo.CryptedName = textCom;
                                RegistrationOrLogin(textCom);
                                client.MyXmlSoft = new XmlSoft(server, client, @"ClientsBase\" + client.MyInfo.CryptedName + @"_Software.xml");
                                client.CaptainTransport.Send(@"1t$OKb@@b1-"); // успешная регистрация
                                WaitCommandInCase = 0;
                                ActiveTransportingMessages = true;
                                TransportProblem = false;
                                break;
                        }
                        break;

                    case 2: // Ждём лист софта
                        switch (command)
                        {
                            default: // Дисконект из-за неверного сообщения
                                client.MyInfo.DateLastOnline = DateTime.Now.ToString();
                                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " --- отправил неверные данные и был отключён [" + client.MyInfo.DateLastOnline + "]");
                                MyServer.TransportMessager.Send("Fu(ck&&DI3-");
                                server.ClientDeListing(client.MyInfo.Guid_id);
                                ActiveTransportingMessages = false;
                                client.Disconnect();
                                break;

                            case @"SenD=Bi4h-": // Получаем софт-массив
                                tempMass[checkMass] = textCom;
                                checkMass++;
                                if (checkMass == 5)
                                {
                                    client.MyXmlSoft.AddApplication(tempMass[0], tempMass[1], tempMass[2], tempMass[3], tempMass[4]);
                                    checkMass = 0;
                                    checkMuss++;
                                }
                                client.CaptainTransport.Send(@"OKb@@b1_g0-" + checkMuss + @"%" + checkMass); // Говорим, что получили и просим на этот раз уже первый чекмас
                                WaitCommandInCase = 2;
                                ActiveTransportingMessages = true;
                                TransportProblem = false;
                                break;

                            case @"SenD=LAST-": // "Это должен быть последний элемент. В противном случае "что-то пошло не так"
                                tempMass[4] = textCom;
                                client.MyXmlSoft.AddApplication(tempMass[0], tempMass[1], tempMass[2], tempMass[3], tempMass[4]);
                                client.MyXmlSoft.Save();
                                client.CaptainTransport.Send(@"it$MaHBi4!-" + DateTime.Now.ToString()); // Благодарим клиента за то, что он няшка и передаём время связи
                                ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " --- Закончил передачу софт-массива [" + DateTime.Now.ToString() + "]");
                                MyServer.MyXmlUser.SoftUp(client.MyInfo.CryptedName, DateTime.Now, checkMuss, @"WhoKnows?");
                                WaitCommandInCase = 0;
                                ActiveTransportingMessages = true;
                                TransportProblem = false;
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }

            void RegistrationOrLogin(string name)
            {
                if (MyServer.MyXmlUser.IsRegistred(client))
                    ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " (" + client.MyInfo.UserIpAdress + ") --- авторизовался в системе [" + DateTime.Now.ToString() + "]");
                else
                    ServerApplication.LogMessageList.Add(client.MyInfo.UserHostName + " --- зарегистрировался в системе [" + client.MyInfo.DateOfRegistration + "]");
            }

            void SendChatToAll(string textChat)
            {
                if (string.Format("{0}: {1}", client.MyInfo.UserHostName, textChat) != string.Format("{0}: {1}", client.MyInfo.UserHostName, null))
                {
                    textChat = string.Format("{0}: {1}", client.MyInfo.UserHostName, textChat);
                    ServerApplication.LogMessageList.Add(textChat);
                    MyServer.TransportMessager.Send(textChat);
                }
            }
        }
    }
}