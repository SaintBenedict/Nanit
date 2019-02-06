
using System;
using System.Net.Sockets;

namespace NaNiT
{
    class ServerCommands
    {
        public static int AwaitVarForCom = 0;
        public static void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            switch (client.AwaitVarForCom)
            {
                case 0: // Не ожидаем команды
                    switch (message)
                    {
                        default: // Просто рандомное сообщение
                            break;
                        case "R33GisSsTrAA1tTiO00oNorLAGINN-": // Команда регистрации или авторизации
                            client.AwaitVarForCom = 1;
                            break;
                        case "CHAT_ALL_P@SSw0oRdd-":
                            client.AwaitVarForCom = 2;
                            Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn);
                            break;
                    }
                    break;
                case 1: // Ожидаем юзернейма для регистрации или авторизации
                    switch (message)
                    {
                        default:
                            Globals.MessageText = message;

                            break;
                        case "R33GisSsTrAA1tTiO00oNorLAGINN-":
                            client.AwaitVarForCom = 1;
                            break;
                    }
                    break;
                case 100: // Ожидание сообщения в админский чат
                    if (String.Format("{0}: {1}", client.userName, message) != String.Format("{0}: {1}", client.userName, null))
                    {

                        message = String.Format("{0}: {1}", client.userName, message);
                        Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn);
                        server.BroadcastMessage(message, client.Id);
                    }
                    else
                    {
                        Stream.Close();
                        message = String.Format("{0}: отключился", client.userName);
                        Globals.MessageText = message;
                        Globals.MessageIn = 100;
                        message = null;
                        if (message != null)
                            server.BroadcastMessage(message, client.Id);
                        else
                            break;

                    }
                    break;
                default:
                    break;
            }
        }
    }
}
