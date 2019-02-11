using System;
using System.Net.Sockets;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class ServerCommands
    {
        public static int AwaitVarForCom = 0;

        public static void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            if (message == "IamStupid-" || message.Length < 10)
            {
                client.dateLastSeen = DateTime.Now.ToString();
                gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                server.RemoveConnection(client.Id);
                client.StupidCheck = true;
                client.Close();
            }
            else
                switch (client.AwaitVarForCom)
                {
                    case 0: // Не ожидаем команды
                        if (message.Length >= 10)
                        {
                            string command = message.Substring(0, 10);
                            string textCom = message.Substring(10, message.Length - 10);
                            switch (command)
                            {
                                default: // Пошёл нахуй, жид пархатый
                                    client.dateLastSeen = DateTime.Now.ToString();
                                    gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                    server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                    server.RemoveConnection(client.Id);
                                    client.StupidCheck = true;
                                    client.Close();
                                    break;

                                case "h@@lLloui-": //Приветствие подключившегося клиента
                                    client.cryptoLogin = textCom;
                                    client.userName = textCom.Substring(0, textCom.Length - 14);
                                    gl_sList_Messages.Add(client.cryptoLogin + " подключился");
                                    // Проверка регистрации
                                    server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self");
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
                            }
                        }
                        break;
                    case 1: // Ожидаем регистрацию
                        if (message.Length >= 10)
                        {
                            string command = message.Substring(0, 10);
                            string textCom = message.Substring(10, message.Length - 10);
                            switch (command)
                            {
                                default: // Пошёл нахуй, жид пархатый
                                    client.dateLastSeen = DateTime.Now.ToString();
                                    gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                    server.BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                    server.RemoveConnection(client.Id);
                                    client.StupidCheck = true;
                                    client.Close();
                                    break;
                                    
                                case "R3GisSsTr-": // Команда регистрации или авторизации
                                    RegistrationOrLogin(textCom);
                                    client.AwaitVarForCom = 0;
                                    client.myMessageNotAwait = false;
                                    client.StupidCheck = false;
                                    break;
                            }
                        }
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }


            void RegistrationOrLogin(string name)
            {
                foreach (string nameOfRegistred in gl_sList_autorisedRegistredClients)
                {
                    if (client.cryptoLogin == name && name == nameOfRegistred)
                    {
                        client.IsRegister = true;
                        gl_sList_Messages.Add(client.userName + " авторизовался в системе [" + DateTime.Now.ToString() + "]");
                        return;
                    }
                }
                if (client.cryptoLogin == name)
                {
                    client.IsRegister = true;
                    gl_sList_autorisedRegistredClients.Add(client.cryptoLogin);
                    client.dateOfRegister = DateTime.Now.ToString();
                    gl_sList_Messages.Add(client.userName + " зарегистрировался в системе [" + client.dateOfRegister + "]");
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
