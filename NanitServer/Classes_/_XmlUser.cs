using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace NaNiT
{
    class _XmlUser : _XmlFile
    {
        public string workRoot = "users";
        public string workSingle = "user";
        public int Records;
        public string[] childs = new string[] { "RegistredDate", "LastSeenDate", "HostName", "IPaddress", "HardwareFile", "SoftwareFile" };
        public int Childs;
        public string[,] Value;

        /// <summary>
        /// XML юзер, работает с пользователями
        /// </summary>
        public _XmlUser() : base("RegistredUsers.xml", "users", "user")
        {
            ReCheck(true);
        }

        /// <summary>
        /// Перерасчёт юзер-массива
        /// </summary>
        public void ReCheck(bool deep = false)
        {
            XmlNodeList _users = xRoot.SelectNodes("*");
            Records = _users.Count;
            Childs = childs.Length;
            if (deep)
            {
                Value = new string[Records, Childs + 1];
                int n = 0, p = 0;
                foreach (XmlNode _user in _users)
                {
                    XmlNode _attr = _user.Attributes.GetNamedItem("name");
                    if (_attr != null)
                    {
                        Value[n, 0] = _attr.Value;
                        if (_user.HasChildNodes)
                        {
                            foreach (XmlNode _params in _user.ChildNodes)
                            {
                                for (p = 0; p < Childs; p++)
                                {
                                    if (_params.Name == childs[p])
                                    {
                                        Value[n, p + 1] = _params.InnerText;
                                    }
                                }
                            }
                        }
                        n++;
                    }
                }
            }
        }

        /// <summary>
        /// Выбор конкретной ноды по названию итема
        /// </summary>
        /// <param name="_name">Имя в аттрибутах ноды</param>
        /// <returns></returns>
        public XmlNode Node(string _name)
        {
            XmlNode temp1 = xRoot.SelectSingleNode(workSingle + @"[@name='" + _name + @"']");
            return temp1;
        }

        /// <summary>
        /// Выбор конкретной ноды по её id
        /// </summary>
        /// <param name="ID">id конкретной ноды</param>
        /// <returns></returns>
        public XmlNode Node(int ID)
        {
            XmlNode temp2 = xRoot.ChildNodes.Item(ID);
            return temp2;
        }

        public bool isRegistred(ClientObject client)
        {
            ReCheck(true);
            for(int i =0; i< Records; i++)
            {
                if(Value[i,0] == client.cryptoLogin)
                {
                    client.idInDatabase = i;
                    if (Node(i).ChildNodes.Count != childs.Length)
                        Repair(i, childs, client.userName);
                    client.IsRegister = true;
                    return true;
                }
            }
            ReCheck();
            client.dateOfRegister = DateTime.Now.ToString();
            client.idInDatabase = Records;
            string[] _client = new string[] { client.dateOfRegister, client.dateOfRegister, client.userName, client.IP,
                                            @"ClientsBase\" + client.cryptoLogin + @"_Hardware.xml", @"ClientsBase\" + client.cryptoLogin + @"_Software.xml" };
            AddMain(workSingle, client.cryptoLogin, childs, _client);
            client.IsRegister = true;
            ReCheck(true);
            return false;
        }

        public void Repair(int id, string[] _childs, string _name)
        {
            int[] temp = new int[_childs.Length];
            foreach(XmlNode tempChild in Node(id))
            {
                
                for(int k=0; k<_childs.Length; k++)
                {
                    if(tempChild.Name == _childs[k])
                        temp[k]++;
                }
            }
            for (int z = 0; z < _childs.Length; z++)
            {
                if (temp[z] == 0)
                    // добавить юзеру {id} чилда [z]
                    temp[z] = 0;
                if (temp[z] > 1)
                    // Сообщить на сервер / записать в логи об ошибке.
                    using (StreamWriter file = new StreamWriter("Users_log.txt", true))
                        {
                            file.WriteLine(DateTime.Now.ToString() + " [Er3.1] ** Зафиксированно задвоение дочернего элемента у пользователя ** [" + id + "] " + _name + @" ||| в элементе " + _childs[z] + " = " + temp[z]);
                        }

            }
        }
    }
}
