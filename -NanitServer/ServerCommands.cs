using System;
using System.Net.Sockets;

namespace NaNiT
{
    class ServerCommands
    {
        public static int AwaitVarForCom = 0;

        public static void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            if (message == "IamStupid-")
            {
                server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self");
                client.AwaitVarForCom = 1;
                client.myMessageNotAwait = true;
                client.StupidCheck = true;
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
                                default: // Просто рандомное сообщение
                                    server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self");
                                    client.AwaitVarForCom = 1;
                                    client.myMessageNotAwait = true;
                                    client.StupidCheck = true;
                                    break;
                                    
                                case "h@@lLloui-": //Приветствие подключившегося клиента
                                    client.cryptoLogin = textCom;
                                    client.userName = textCom.Substring(0, textCom.Length - 14);
                                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, client.cryptoLogin + " подключился");
                                    // Проверка регистрации
                                    server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self");
                                    client.AwaitVarForCom = 1;
                                    client.myMessageNotAwait = true;
                                    break;
                                    
                                case "CH@T_AlL_-": // Команда админской рассылки мессейджа
                                    SendChatToAll(textCom);
                                    client.AwaitVarForCom = 0;
                                    client.myMessageNotAwait = false;
                                    break;
                                    
                                case "i_C@N_Y0U-": // Мессага о подключении клиента после обрыва
                                    ReconnectUser(client);
                                    client.AwaitVarForCom = 0;
                                    client.myMessageNotAwait = false;
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
                                    server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self");
                                    client.AwaitVarForCom = 1;
                                    client.myMessageNotAwait = true;
                                    client.StupidCheck = true;
                                    break;
                                    
                                case "R3GisSsTr-": // Команда регистрации или авторизации
                                    RegistrationOrLogin(textCom);
                                    client.AwaitVarForCom = 0;
                                    client.myMessageNotAwait = false;
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
                foreach (string nameOfRegistred in Globals.AutorisedRegistredClients)
                {
                    if (client.cryptoLogin == name && name == nameOfRegistred)
                    {
                        client.IsRegister = true;
                        Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, client.userName + " авторизовался в системе " + DateTime.Now.ToString());
                        return;
                    }
                }
                if (client.cryptoLogin == name)
                {
                    client.IsRegister = true;
                    Globals.AutorisedRegistredClients.Add(client.cryptoLogin);
                    client.dateOfRegister = DateTime.Now.ToString();
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, client.userName + " зарегистрировался в системе " + client.dateOfRegister);
                }
            }
            void SendChatToAll(string textChat)
            {
                if (String.Format("{0}: {1}", client.userName, textChat) != String.Format("{0}: {1}", client.userName, null))
                {
                    textChat = String.Format("{0}: {1}", client.userName, textChat);
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, textChat);
                    server.BroadcastMessage(textChat, ServerObject.clients, client, "all");
                }
            }
            void ReconnectUser(ClientObject ReconClient)
            {
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, client.userName + " Подключение восстановлено после разрыва " + DateTime.Now.ToString());
            }
        }
    }
}
