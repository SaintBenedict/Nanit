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
                server.BroadcastMessage("@HowdyHu%$-", client.Id, ServerObject.clients, client, "self");
                client.AwaitVarForCom = 1;
                client.myMessageNotAwait = true;
                client.StupidCheck = true;
            }
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
                                client.myMessageNotAwait = false;
                                break;
                            case "h@@lLloui-": //Приветствие подключившегося клиента
                                client.userName = textCom;
                                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, client.userName + " подключился");
                                // Проверка регистрации
                                server.BroadcastMessage("@HowdyHu%$-", client.Id, ServerObject.clients, client, "self");
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
                                client.Close();
                                client.myMessageNotAwait = false;
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
                string TempTime;
                string newMessage;
                foreach (string nameOfRegistred in Globals.AutorisedRegistredClients)
                {
                    if (client.userName == name)
                    {
                        if (client.userName == nameOfRegistred)
                        {
                            client.IsRegister = true;
                            if (client.dateLastSeen != null)
                                TempTime = client.dateLastSeen;
                            else
                                TempTime = "Не зафиксировано";
                            newMessage = client.userName + " авторизовался в системе " + DateTime.Now.ToString() + " Последнее появление " + TempTime;
                            Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, newMessage);
                            return;
                        }
                    }
                }
                if (client.userName == name)
                {
                    client.IsRegister = true;
                    Globals.AutorisedRegistredClients.Add(client.userName);
                    client.dateOfRegister = DateTime.Now.ToString();
                    newMessage = client.userName + " зарегистрировался в системе " + client.dateOfRegister;
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, newMessage);
                }
            }
            void SendChatToAll(string textChat)
            {
                if (String.Format("{0}: {1}", client.userName, textChat) != String.Format("{0}: {1}", client.userName, null))
                {

                    textChat = String.Format("{0}: {1}", client.userName, textChat);
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, textChat);
                    server.BroadcastMessage(textChat, client.Id, ServerObject.clients, client, "all");
                }
                else
                {
                    Stream.Close();
                    textChat = String.Format("{0}: отключился", client.userName);
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, textChat);
                }
            }
            void ReconnectUser(ClientObject ReconClient)
            {
                string TempMessage = client.userName + " Подключение восстановлено после разрыва " + DateTime.Now.ToString();
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, TempMessage);
            }
        }
    }
}
