using System.Xml;

namespace NaNiT
{
    class MyXml
    {
        public string NameThisXmlFile;
        public XmlDocument ThisXmlFile;
        public XmlElement ThisXmlRoot;
        public string[,,,] Value;
        public string[] NameNodes;
        public string[] SubNodes;
        public string[] SubSubNodes;
        public int NameItems;
        public int NameValue;
        public int SubItems;
        public int SubSubItems;
        public string NameNode;

        /*Если у нас XML с юзерами, то [ 0, x , 0 , 0 ] это имя пользователя, метод .NameValue это их количество
         [ 0 , x , y , 0 ] тут Y передаёт параметр, 1 - РегДата, 2 - ЛастСин, 3 - ХостНэйм, 4 - IP, 5 - Группа, 6 - файл с железом, 7 - файл с софтом. */

        public MyXml(string NameXmlFile)
        {
            NameThisXmlFile = NameXmlFile;
            if (System.IO.File.Exists(NameThisXmlFile))
                ReopenXml();
            else
            {
                if (NameThisXmlFile.Substring(NameThisXmlFile.Length - 18, 18) == "RegistredUsers.xml")
                {
                    CreateXML(NameThisXmlFile, "users");
                    ReopenXml();
                }
                if (NameThisXmlFile.Substring(NameThisXmlFile.Length - 12, 12) == "Software.xml")
                {
                    CreateXML(NameThisXmlFile, "softwares");
                    ReopenXml();
                }
            }
        }

        public void ReopenXml()
        {
            ThisXmlFile = new XmlDocument();
            ThisXmlFile.Load(NameThisXmlFile);
            // получим корневой элемент
            ThisXmlRoot = ThisXmlFile.DocumentElement;
            switch (ThisXmlRoot.Name)
            {
                case "users":
                    NameNode = "user";
                    NameNodes = new string[] { "name" };
                    NameItems = NameNodes.Length;
                    SubNodes = new string[] { "name", "RegistredDate", "LastSeenDate", "HostName", "IPaddress", "CorporateGroup", "HardwareFile", "SoftwareFile" }; // users
                    SubItems = SubNodes.Length;
                    SubSubNodes = null;
                    SubSubItems = 0;
                    break;
                case "softwares":
                    NameNode = "application";
                    NameNodes = new string[] { "name" };
                    NameItems = NameNodes.Length;
                    SubNodes = new string[] { "name", "Version", "Publisher", "InstallDate", "InstallLocation" }; // applications
                    SubItems = SubNodes.Length;
                    SubSubNodes = null;
                    SubSubItems = 0;
                    break;
            }
            XmlNodeList NameList = ThisXmlRoot.SelectNodes("*");
            NameValue = NameList.Count;
            Value = new string[NameItems, NameValue, SubItems, SubSubItems + 1];

            int f = 0, n = 0, s = 0, l = 0;

            for (f = 0; f < NameItems; f++)
            {
                foreach (XmlNode tempNameNode in NameList)
                {
                    XmlNode attr = tempNameNode.Attributes.GetNamedItem(NameNodes[f]);
                    if (attr != null)
                    {
                        Value[f, n, 0, 0] = attr.Value;

                        if (tempNameNode.HasChildNodes)
                        {
                            foreach (XmlNode tempSubNode in tempNameNode.ChildNodes)
                            {
                                for (s = 1; s < SubItems; s++)
                                {
                                    if (tempSubNode.Name == SubNodes[s])
                                    {
                                        Value[f, n, s, l] = tempSubNode.InnerText;

                                        if (tempSubNode.HasChildNodes)
                                        {
                                            foreach (XmlNode tempSubSubNode in tempSubNode.ChildNodes)
                                            {
                                                for (l = 0; l < SubSubItems; l++)
                                                {
                                                    if (tempSubSubNode.Name == SubSubNodes[l])
                                                    {
                                                        Value[f, n, s, l + 1] = tempSubSubNode.InnerText;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            n++;
                        }
                    }

                }
            }
        }

        public void AddUser(ClientObject client)
        {
            XmlDocument ThisXmlFile = new XmlDocument();
            ThisXmlFile.Load(NameThisXmlFile);
            XmlElement ThisXmlRoot = ThisXmlFile.DocumentElement;
            // создаем новый элемент user
            XmlElement userElem = ThisXmlFile.CreateElement("user");
            // создаем атрибут name
            XmlAttribute nameAttr = ThisXmlFile.CreateAttribute("name");
            // создаем элементы необходимые файлу
            XmlElement regDateElem = ThisXmlFile.CreateElement("RegistredDate");
            XmlElement lastSeenElem = ThisXmlFile.CreateElement("LastSeenDate");
            XmlElement hostNameElem = ThisXmlFile.CreateElement("HostName");
            XmlElement ipAdressElem = ThisXmlFile.CreateElement("IPaddress");
            XmlElement corpGroupElem = ThisXmlFile.CreateElement("CorporateGroup");
            XmlElement hardwareFileElem = ThisXmlFile.CreateElement("HardwareFile");
            XmlElement softwareFileElem = ThisXmlFile.CreateElement("SoftwareFile");

            // создаем текстовые значения для элементов и атрибута
            XmlText nameText = ThisXmlFile.CreateTextNode(client.CryptedName);
            XmlText regDateText = ThisXmlFile.CreateTextNode(client.DateOfRegistration);
            XmlText lastSeenText = ThisXmlFile.CreateTextNode(client.DateOfRegistration);
            XmlText hostNameText = ThisXmlFile.CreateTextNode(client.UserHostName);
            XmlText ipAdressText = ThisXmlFile.CreateTextNode(client.UserIpAdress);
            XmlText hardwareFileText = ThisXmlFile.CreateTextNode(@"ClientsBase\" + client.CryptedName + @"_Hardware.xml");
            XmlText softwareFileText = ThisXmlFile.CreateTextNode(@"ClientsBase\" + client.CryptedName + @"_Software.xml");

            //добавляем узлы
            nameAttr.AppendChild(nameText);
            regDateElem.AppendChild(regDateText);
            lastSeenElem.AppendChild(lastSeenText);
            hostNameElem.AppendChild(hostNameText);
            ipAdressElem.AppendChild(ipAdressText);
            hardwareFileElem.AppendChild(hardwareFileText);
            softwareFileElem.AppendChild(softwareFileText);

            userElem.Attributes.Append(nameAttr);
            userElem.AppendChild(regDateElem);
            userElem.AppendChild(lastSeenElem);
            userElem.AppendChild(hostNameElem);
            userElem.AppendChild(ipAdressElem);
            userElem.AppendChild(corpGroupElem);
            userElem.AppendChild(hardwareFileElem);
            userElem.AppendChild(softwareFileElem);

            ThisXmlRoot.AppendChild(userElem);
            ThisXmlFile.Save(NameThisXmlFile);

            CreateXML(@"ClientsBase\" + client.CryptedName + @"_Hardware.xml", "hardwares");
            CreateXML(@"ClientsBase\" + client.CryptedName + @"_Software.xml", "softwares");
        }

        // Создание нового XML файла
        public void CreateXML(string newName, string sampleClass)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.RemoveAll();
            xmldoc.AppendChild(xmldoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement ElmntRoot = xmldoc.CreateElement(sampleClass);
            xmldoc.AppendChild(ElmntRoot);
            xmldoc.Save(newName);
        }

        // Запись конкретного нода в юзерс.xml
        public void WriteTo(string NodeName, string NewVar, int idUser)
        {
            string[] tempToNode = SubNodes;
            int x = 0;
            for (int n = 0; n < SubNodes.Length; n++)
            {
                if (SubNodes[n] == NodeName)
                {
                    x = n;
                    break;
                }
            }
            ThisXmlFile = new XmlDocument();
            ThisXmlFile.Load(NameThisXmlFile);
            // получим корневой элемент
            ThisXmlRoot = ThisXmlFile.DocumentElement;
            XmlNode temp1 = ThisXmlFile.DocumentElement.ChildNodes.Item(idUser);
            XmlNode nodeToChange = temp1.ChildNodes.Item(x - 1);
            nodeToChange.InnerText = NewVar;
            Value[0, idUser, x, 0] = NewVar;
            ThisXmlFile.Save(NameThisXmlFile);
        }

        // Добавление софта
        public void AddApplication(string Item, string Version, string Publisher, string Date, string Location)
        {
            // создаем новый элемент user
            XmlElement tempApp = ThisXmlFile.CreateElement("application");
            // создаем атрибут name
            XmlAttribute tempName = ThisXmlFile.CreateAttribute("name");
            // создаем элементы необходимые файлу
            XmlElement tempVer = ThisXmlFile.CreateElement("Version");
            XmlElement tempPublish = ThisXmlFile.CreateElement("Publisher");
            XmlElement tempInDate = ThisXmlFile.CreateElement("InstallDate");
            XmlElement tempInLoc = ThisXmlFile.CreateElement("InstallLocation");

            XmlText itemText = ThisXmlFile.CreateTextNode(Item);
            XmlText versText = ThisXmlFile.CreateTextNode(Version);
            XmlText publText = ThisXmlFile.CreateTextNode(Publisher);
            XmlText inDateText = ThisXmlFile.CreateTextNode(Date);
            XmlText inLocaText = ThisXmlFile.CreateTextNode(Location);
            tempName.AppendChild(itemText);
            tempVer.AppendChild(versText);
            tempPublish.AppendChild(publText);
            tempInDate.AppendChild(inDateText);
            tempInLoc.AppendChild(inLocaText);

            tempApp.Attributes.Append(tempName);
            tempApp.AppendChild(tempVer);
            tempApp.AppendChild(tempPublish);
            tempApp.AppendChild(tempInDate);
            tempApp.AppendChild(tempInLoc);
            ThisXmlRoot.AppendChild(tempApp);
        }
        public void SaveThis()
        {
            ThisXmlFile.Save(NameThisXmlFile);
        }
    }
}

