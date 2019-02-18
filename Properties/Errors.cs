using System;
using System.IO;

namespace NaNiT
{
    public static class Error
    {
        /// <summary>
        /// Первая буква Е обозначает ошибку. Вторая относится к типу ошибки (D - data, P - programm, C - connection), далее цифра обозначающая условный приоритет ошибок, где 0 - критическая.
        /// Следующие две буквы указывают на класс вызвавший ошибку. Последняя цифра порядковый номер ошибки внутри класса.
        /// Параметры везде свои, зависит от конкретной ошибки, но везде строковые
        /// </summary>
        public static void Msg(string ErrorCode, params string[] param)
        {
            string _n = DateTime.Now.ToString() + " [" + ErrorCode + "] ** ", _f = "", _d = " ** ";
            string message, _m;
            _f = "Сообщение обработчика: " + param[0]; // Стандартный F для одного параметра от обработчика catch
            switch (ErrorCode)
            {
                default:
                    _m = "ГЛОБАЛЬНАЯ ОШИБКА. Ошибка в том, что неопознан код ошибки. Ебать вы долбоёбы, ребята, так криво программу писать.";
                    break;
                    
                #region XmL_Working [E**Xm***]
                case "ED4Xm2.1":
                    _m = "Зафиксированно задвоение дочернего элемента у пользователя";
                    _f = "[" + param[0] + "] " + param[1] + @" ||| в элементе - " + param[2] + " = " + param[3];
                    break;
                case "ES2Xm3.2":
                    _m = "Зафиксирована смена ip адреса у компьютера";
                    _f = "[" + param[0] + "] " + param[1] + @" ||| новый ip адрес - " + param[2] + @"(- " + param[3] + ")";
                    break;
                case "ED3Xm3.3":
                    _m = "Зафиксирована смена имени компьютера у пользователя";
                    _f = "[" + param[0] + "] " + param[1] + @" ||| новое имя компьютера - " + param[2];
                    break;
                #endregion

                #region FormSOptions [E**Fm***]
                case "EP1Fm0.1":
                    _m = "Фоновая обработка списка внезапно прекращена";
                    _f = "Сообщение обработчика: " + param[0];
                    break;
                case "EP3Fm0.2":
                    _m = "В лог лист сообщений не добавилось сообщение, вызвав ошибку";
                    break;
                #endregion

                #region ServerObject [E**Li***]
                case "EP0Li0.1":
                    _m = "Ошибка TcpListener при старте сервера";
                    break;
                case "EP0Li1.1":
                    _m = "Ошибка при создании нового подключения при Accept из Listener";
                    break;
                case "EP0Dc1.1":
                    _m = "Ошибка во время отключения сервера";
                    break;
                #endregion

                #region CommandManager [E**Tr***]
                case "EP0Tr2.1":
                    _m = "Транспортная ошибка при отправке сообщения";
                    break;
                #endregion

                #region ClientObject [E**Cl***]
                case "ED1Cl1.1":
                    _m = "Ошибка при добавлении нового клиента в список активных пользователей";
                    break;
                case "ED1Cl2.1":
                    _m = "Неудачное удаление пользователя из списка";
                    break;
                case "EP1Cl2.1":
                    _m = "Ошибка при отключении клиента";
                    break;
                case "EP1Cl1.1":
                    _m = "Ошибка при получении сообщения";
                    break;
                    #endregion
            }

            message = _n + _m + _d + _f;
            using (StreamWriter file = new StreamWriter("Server_log.txt", true))
            {
                file.WriteLine(message);
            }
            _n = null; _m = null; _f = null; message = null;
        }
    }
}
