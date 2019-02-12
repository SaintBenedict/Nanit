using System;
using System.Net.Sockets;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class ServerCommands
    {
        public static int AwaitVarForCom = 0;
        public static int SoftwareMassive = 0;
        public static string[] tempMass = new string[3];
        public static int checkMass = 0;

        public static void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            if (message == "IamStupid-" || message.Length < 10)
            {
                client.dateLastSeen = DateTime.Now.ToString();
                gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                server.RemoveConnection(client.Id);
                client.StupidCheck = true;
                client.myMessageNotAwait = false;
                client.Close();
            }
            else
            {
                string command = message.Substring(0, 10);
                string textCom = message.Substring(10, message.Length - 10);
                switch (client.AwaitVarForCom)
                {

                    case 0: // Не ожидаем команды
                        switch (command)
                        {
                            default: // Пошёл нахуй, жид пархатый
                                client.dateLastSeen = DateTime.Now.ToString();
                                gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                server.RemoveConnection(client.Id);
                                client.StupidCheck = true;
                                client.myMessageNotAwait = false;
                                client.Close();
                                break;

                            case "h@@lLloui-": //Приветствие подключившегося клиента
                                client.cryptoLogin = textCom;
                                client.userName = textCom.Substring(0, textCom.Length - 14);
                                gl_sList_Messages.Add(client.cryptoLogin + " --- подключился");
                                // Проверка регистрации
                                server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self"); // спрашиваем кто он такой блин
                                client.AwaitVarForCom = 1;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;

                            case "CH@T_AlL_-": // Команда админской рассылки мессейджа
                                SendChatToAll(textCom);
                                client.AwaitVarForCom = 0;
                                client.myMessageNotAwait = false;
                                client.StupidCheck = false;
                                break;

                            case "SenD=S0fT-": // Божечки, да нас массивом софта завалят сейчас
                                client.AwaitVarForCom = 2;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                SoftwareMassive = Convert.ToInt32(textCom);
                                client.softXmlBase = new MyXml(@"ClientsBase\" + client.cryptoLogin + @"_Software.xml");
                                server.BroadcastMessage("OKb@@b1_g0-"+ checkMass, ServerObject.clients, client, "self"); // говорим, что готовы въёбывать
                                break;
                        }
                        break;

                    case 1: // Ожидаем регистрацию
                        switch (command)
                        {
                            default: // Пошёл нахуй, жид пархатый
                                client.dateLastSeen = DateTime.Now.ToString();
                                gl_sList_Messages.Add(client.userName + " --- отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                server.RemoveConnection(client.Id);
                                client.StupidCheck = true;
                                client.myMessageNotAwait = false;
                                client.Close();
                                break;

                            case "R3GisSsTr-": // Команда регистрации или авторизации
                                RegistrationOrLogin(textCom);
                                server.BroadcastMessage("1t$OKb@@b1-", ServerObject.clients, client, "self"); // успешная регистрация
                                client.AwaitVarForCom = 1;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;
                        }
                        break;

                    case 2: // Ждём лист софта
                        switch (command)
                        {
                            default: // Пошёл нахуй, жид пархатый
                                client.dateLastSeen = DateTime.Now.ToString();
                                gl_sList_Messages.Add(client.userName + " --- отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                server.RemoveConnection(client.Id);
                                client.StupidCheck = true;
                                client.myMessageNotAwait = false;
                                client.Close();
                                break;

                            case "SenD=Bi4h-": // Команда регистрации или авторизации
                                tempMass[checkMass] = textCom;
                                checkMass++;
                                if (checkMass == 3)
                                {
                                    client.softXmlBase.AddApplication(tempMass[0], tempMass[1], tempMass[2]);
                                    checkMass = 0;

                                }
                                server.BroadcastMessage("OKb@@b1_g0-" + checkMass, ServerObject.clients, client, "self"); // успешная регистрация
                                client.AwaitVarForCom = 2;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }

            void RegistrationOrLogin(string name)
            {
                for (int q = 0; q < gl_xml_users.NameValue; q++)
                {
                    if (client.cryptoLogin == gl_xml_users.Value[0, q, 0, 0])
                    {
                        client.IsRegister = true;
                        client.idInDatabase = q;
                        gl_sList_Messages.Add(client.userName + " (" + client.IP + ") --- авторизовался в системе [" + DateTime.Now.ToString() + "]");
                        gl_xml_users.WriteTo("LastSeenDate", DateTime.Now.ToString(), client.idInDatabase);
                        gl_xml_users.WriteTo("IPaddress", client.IP, client.idInDatabase);
                        return;
                    }
                }
                if (client.cryptoLogin == name)
                {
                    client.IsRegister = true;
                    client.dateOfRegister = DateTime.Now.ToString();
                    client.idInDatabase = gl_xml_users.NameValue + 1;
                    gl_xml_users.AddUser(client);
                    gl_xml_users.ReopenXml();
                    gl_sList_Messages.Add(client.userName + " --- зарегистрировался в системе [" + client.dateOfRegister + "]");
                }
            }

            void SendChatToAll(string textChat)
            {
                if (String.Format("{0}: {1}", client.userName, textChat) != String.Format("{0}: {1}", client.userName, null))
                {
                    textChat = String.Format("{0}: {1}", client.userName, textChat);
                    gl_sList_Messages.Add(textChat);
                    server.BroadcastMessage(textChat, ServerObject.clients, client, "all");
                }
            }
        }
    }
}
