using System.Xml;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class OptionsXml
    {
        public static void ReArrayNodes()
        {
            gl_sMas_nameNodes = new string[] { "name", "RegistredDate", "LastSeenDate", "HostName", "IPadress", "CorporateGroup", "HardwareFile", "SoftwareFile" }; // users
        }
    }
    class MyXml
    {
        protected internal string NameThisXmlFile;
        protected internal XmlDocument ThisXmlFile;
        protected internal XmlElement ThisXmlRoot;
        protected internal string[,] Value;


        public MyXml(string NameXmlFile)
        {
            NameThisXmlFile = NameXmlFile;
            ThisXmlFile = new XmlDocument();
            ThisXmlFile.Load(NameXmlFile + ".xml");
            // получим корневой элемент
            ThisXmlRoot = ThisXmlFile.DocumentElement;
            string[] TempNode = gl_sMas_nameNodes;
            switch (ThisXmlRoot.Value)
            {
                case "users":
                    TempNode = gl_sMas_nameNodes;
                    break;
            }
            int items = 0;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in ThisXmlRoot)
            {
                items++;
            }
            Value = new string[items, TempNode.Length];
            int i = 0;
            foreach (XmlNode xnode in ThisXmlRoot)
            {
                // получаем атрибут name
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem(TempNode[0]);
                    if (attr != null)
                    {
                        Value[i, 0] = attr.Value;
                        i++;
                    }
                }
                // обходим все дочерние узлы элемента user
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    for (int k = 1; k < TempNode.Length; k++)
                    {
                        // если узел - company
                        if (childnode.Name == TempNode[k])
                        {
                            Value[i-1, k] = childnode.InnerText;
                        }
                    }
                }
            }
        }
    }
}
