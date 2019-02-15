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
            string _n = DateTime.Now.ToString() + " [" + ErrorCode + "] ** ";
            string message, _m;
            string _f = "";
            switch (ErrorCode)
            {
                default:
                    _m = "ГЛОБАЛЬНАЯ ОШИБКА. Ошибка в том, что неопознан код ошибки. Ебать вы долбоёбы, ребята, так криво программу писать.";
                    break;
                case "ED4Xm2.1":
                    _m = "Зафиксированно задвоение дочернего элемента у пользователя";
                    _f = " ** [" + param[0] + "] " + param[1] + @" ||| в элементе - " + param[2] + " = " + param[3];
                    break;
                case "ES2Xm3.2":
                    _m = "Зафиксирована смена ip адреса у компьютера";
                    _f = " ** [" + param[0] + "] " + param[1] + @" ||| новый ip адрес - " + param[2] + @"(- " + param[3] + ")";
                    break;
                case "ED3Xm3.3":
                    _m = "Зафиксирована смена имени компьютера у пользователя";
                    _f = " ** [" + param[0] + "] " + param[1] + @" ||| новое имя компьютера - " + param[2];
                    break;
            }

            message = _n + _m + _f;
            using (StreamWriter file = new StreamWriter("Users_log.txt", true))
            {
                file.WriteLine(message);
            }
            _n = null; _m = null; _f = null; message = null;
        }
    }
}
