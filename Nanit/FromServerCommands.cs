namespace NaNiT
{
    class FromServerCommands
    {
        public static void DoWithServerCommand(string message)
        {
            if (message == "Fu(ck&&DI3-")
            {

            }
            else
                switch (Globals.AwaitVarForCom)
                {
                    case 0: // Ожидаем запрос на регистрацию
                        if (message.Length >= 11)
                        {
                            string command = message.Substring(0, 11);
                            string textCom = message.Substring(11, message.Length - 11);
                            switch (command)
                            {
                                default: // хуйня какая-то
                                    CFunc.SendMessage("IamStupid-");
                                    Globals.AwaitVarForCom = 0;
                                    Globals.myMessageNotAwait = true;
                                    break;
                                case "@HowdyHu%$-": // Команда регистрации или авторизации
                                    CFunc.SendMessage("R3GisSsTr-" + Globals.userName);
                                    Globals.AwaitVarForCom = 0;
                                    Globals.myMessageNotAwait = false;
                                    break;
                            }
                        }
                        break;
                    case 1: // Ожидаем регистрацию
                        if (message.Length >= 11)
                        {
                            string command = message.Substring(0, 11);
                            string textCom = message.Substring(11, message.Length - 11);
                            switch (command)
                            {
                                default: // хуйня какая-то
                                    CFunc.SendMessage("IamStupid-");
                                    Globals.AwaitVarForCom = 0;
                                    Globals.myMessageNotAwait = true;
                                    break;
                                case "": // 
                                    break;
                            }
                        }
                        break;
                    default: // Типа если сообщение меньше 11-ти символов?
                        CFunc.SendMessage("IamStupid-");
                        Globals.AwaitVarForCom = 0;
                        Globals.myMessageNotAwait = true;
                        break;
                }
        }
    }
}