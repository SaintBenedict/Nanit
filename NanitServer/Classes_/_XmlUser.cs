using System;
using System.Xml;

namespace NaNiT
{
    class _XmlUser : _XmlFile
    {
        /// <summary>
        /// Массив/перечень под-свойств юзера
        /// </summary>
        public string[] childs { get; } = new string[] { "RegistredDate", "LastSeenDate", "HostName", "IPaddress", "HardwareFile", "SoftwareFile" };
        /// <summary>
        /// Массив со всеми значениями юзеров
        /// </summary>
        public string[,] Value { get; set; }

        /// <summary>
        /// XML юзер, работает с пользователями
        /// </summary>
        public _XmlUser() : base("RegistredUsers.xml", "users", "user")
        {
            ReCheck(true);
            Sorting();
        }

        /// <summary>
        /// Перерасчёт количества юзеров
        /// </summary>
        /// <param name="deep">И всего массива значений Value</param>
        public void ReCheck(bool deep = false)
        {
            Value = new string[xRoot.SelectNodes("*").Count, childs.Length + 1];
            int n = 0, p = 0;
            foreach (XmlNode _user in xRoot.SelectNodes("*"))
            {
                XmlNode _attr = _user.Attributes.GetNamedItem("name");
                if (_attr != null)
                {
                    Value[n, 0] = _attr.Value;
                    if (_user.HasChildNodes)
                    {
                        foreach (XmlNode _params in _user.ChildNodes)
                        {
                            for (p = 0; p < childs.Length; p++)
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

        /// <summary>
        /// Выбор конкретной ноды по названию итема
        /// </summary>
        /// <param name="_name">Имя в аттрибутах ноды</param>
        /// <returns></returns>
        public XmlNode Node(string _name)
        {
            XmlNode temp1 = xRoot.SelectSingleNode("user" + @"[@name='" + _name + @"']");
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

        /// <summary>
        /// Проверяет наличие Клиента в базе и регистрирует его если нет.
        /// </summary>
        /// <param name="client">Клиент подключившийся к серверу</param>
        public bool isRegistred(ClientObject client)
        {
            // Забиваем массив данными, который пойдут в файл при регистрации, либо после авторизации для проверки
            string[] _client = new string[] { DateTime.Now.ToString(), DateTime.Now.ToString(), client.userName, client.IP,
                                            @"ClientsBase\" + client.cryptoLogin + @"_Hardware.xml", @"ClientsBase\" + client.cryptoLogin + @"_Software.xml" };
            ReCheck(true);
            for (int i = 0; i < xRoot.SelectNodes("*").Count; i++)
            {
                if (Value[i, 0] == client.cryptoLogin)
                {
                    client.idInDatabase = i;
                    // Тут мы проверяем текущие ноды в файле на соответствие с параметрами программы, чтобы добавить новые (после обновления версии если появятся)
                    // Или сообщить администратору о задвоении
                    if (Node(i).ChildNodes.Count != childs.Length)
                        Repair(i);
                    Edit(i, 1, DateTime.Now.ToString());
                    if (Value[i, 3] != client.IP)
                    {
                        Error.Msg("ES2Xm3.2", i.ToString(), Value[i,0], client.IP, Value[i,3]);
                        Edit(i, 3, client.IP);
                    }
                    if (Value[i, 2] != client.userName)
                    {
                        Error.Msg("ED3Xm3.3", i.ToString(), Value[i, 0], client.userName);
                        Edit(i, 2, client.userName);
                    }
                    client.IsRegister = true;
                    return true;
                }
            }
            ReCheck();
            // Если клиент не зарегистрирован, то забиваем новый массив данными, которые пойдут в его потомков (параметры)
            client.dateOfRegister = DateTime.Now.ToString();
            client.idInDatabase = xRoot.SelectNodes("*").Count;
            // И добавляем его в xml файл (сохранение файла идёт в конце AddMain метода)
            AddMain("user", client.cryptoLogin, childs, _client);
            client.IsRegister = true;
            ReCheck(true);
            return false;

            /// <summary>
            /// Исправляет у клиента отсутствующие чилд.ноды
            /// </summary>
            /// <param name="id">id клиента в базе xml</param>
            void Repair(int id)
            {
                int[] temp = new int[childs.Length];
                foreach (XmlNode tempChild in Node(id))
                {
                    // Пересчитываем количество совпадений имени ноды и параметра
                    for (int k = 0; k < childs.Length; k++)
                    {
                        if (tempChild.Name == childs[k])
                            temp[k]++;
                    }
                }
                for (int z = 0; z < childs.Length; z++)
                {
                    // И выполняем одно из двух действий, если какая-то нода отсутствует или задваивается
                    if (temp[z] == 0)
                        // добавить юзеру {id} чилда [z]
                        Restore(z);
                    if (temp[z] > 1)
                        Error.Msg("ED4Xm2.1", id.ToString(), client.userName, childs[z], temp[z].ToString());
                }

                /// <summary>
                /// Добавляем отсутствующего потомка в конец списка (затем отдельным методом сортировка)
                /// </summary>
                /// <param name="z">Порядковый номер отсутствующего потомка</param>
                void Restore(int z)
                {
                    XmlNode pure = Node(id);
                    XmlElement pureChild = xDoc.CreateElement(childs[z]);
                    XmlText pureText = xDoc.CreateTextNode(_client[z]);
                    pureChild.AppendChild(pureText);
                    pure.AppendChild(pureChild);
                    Sorting(pure);
                    Save();
                }
            }
        }

        /// <summary>
        /// Изменяет в текущем файле Xml определённые значение параметров пользователя
        /// </summary>
        /// <param name="id">id клиента в базе xml</param>
        /// <param name="child">Порядковый номер изменяемого ребёнка</param>
        /// <param name="_value">Новое значение</param>
        public void Edit(int id, int child, string _value)
        {
            foreach (XmlNode tempChild in Node(id))
            {
                if (tempChild.Name == childs[child])
                {
                    tempChild.InnerText = _value;
                    Value[id, child] = _value;
                    Save();
                }
            }
        }

        /// <summary>
        /// Сортировка параметров конкретного элемента в рамках одной ноды
        /// </summary>
        /// <param name="_userSort">Нода, детей которой сортируем</param>
        public void Sorting(XmlNode _userSort)
        {
            XmlNode firstChild = _userSort.SelectSingleNode(childs[0]);
            _userSort.PrependChild(firstChild);
            for (int i = 1; i < childs.Length; i++)
            {
                XmlNode childToMove = _userSort.SelectSingleNode(childs[i]);
                if (childToMove == null)
                {
                    XmlElement newEl = xDoc.CreateElement(childs[i]);
                    _userSort.AppendChild(newEl);
                    childToMove = newEl;
                }
                _userSort.InsertAfter(childToMove, _userSort.ChildNodes[i]);
            }
            Save();
        }
        /// <summary>
        /// Сортировка параметров всех элементов всего документа
        /// </summary>
        public void Sorting()
        {
            XmlNodeList _users = xRoot.SelectNodes("*");
            foreach (XmlNode sort in _users)
                Sorting(sort);
            Save();
        }

        public void SoftUp(ClientObject client, DateTime _last, int _elems, string isChanged)
        {
            foreach (XmlNode tempChild in Node(client.cryptoLogin))
            {
                if (tempChild.Name == "SoftwareFile")
                {
                    XmlElement SoftUp = xDoc.CreateElement("Uppload");
                    XmlAttribute md5 = xDoc.CreateAttribute("md5_soft");
                    XmlAttribute number = xDoc.CreateAttribute("soft_count");
                    XmlAttribute change = xDoc.CreateAttribute("soft_changed");
                    XmlAttribute newSoftUp = xDoc.CreateAttribute("soft_uppload");
                    XmlText md5Text = xDoc.CreateTextNode("Тут будет МД5 выборка по текущему софту");
                    XmlText numberText = xDoc.CreateTextNode(_elems.ToString());
                    XmlText changeText = xDoc.CreateTextNode("no");
                    XmlText upploadText = xDoc.CreateTextNode(_last.ToString());
                    md5.AppendChild(md5Text);
                    number.AppendChild(numberText);
                    change.AppendChild(changeText);
                    newSoftUp.AppendChild(newSoftUp);
                    SoftUp.Attributes.Append(md5);
                    SoftUp.Attributes.Append(number);
                    SoftUp.Attributes.Append(change);
                    SoftUp.Attributes.Append(change);
                    tempChild.AppendChild(newSoftUp);
                    tempChild.PrependChild(newSoftUp);
                        
                Clean:
                    if (tempChild.ChildNodes.Count > 3)
                    {
                        tempChild.RemoveChild(tempChild.LastChild);
                        goto Clean;
                    }
                    Save();
                }
            }
        }

        public void HardUp(ClientObject client, DateTime _last, string isChanged)
        {
            foreach (XmlNode tempChild in Node(client.cryptoLogin))
            {
                if (tempChild.Name == "HardwareFile")
                {
                    XmlElement HardUp = xDoc.CreateElement("Uppload");
                    XmlAttribute md5 = xDoc.CreateAttribute("md5_hard");
                    XmlAttribute change = xDoc.CreateAttribute("soft_changed");
                    XmlAttribute newHardUp = xDoc.CreateAttribute("hard_uppload");
                    XmlText md5Text = xDoc.CreateTextNode("Тут будет МД5 выборка по текущему софту");
                    XmlText changeText = xDoc.CreateTextNode("no");
                    XmlText upploadText = xDoc.CreateTextNode(_last.ToString());
                    md5.AppendChild(md5Text);
                    change.AppendChild(changeText);
                    newHardUp.AppendChild(upploadText);
                    HardUp.Attributes.Append(md5);
                    HardUp.Attributes.Append(change);
                    HardUp.Attributes.Append(newHardUp);
                    tempChild.AppendChild(newHardUp);
                    tempChild.PrependChild(newHardUp);

                Clean:
                    if (tempChild.ChildNodes.Count > 3)
                    {
                        tempChild.RemoveChild(tempChild.LastChild);
                        goto Clean;
                    }
                    Save();
                }
            }
        }
    }
}
