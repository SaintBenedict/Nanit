using System.Xml;

namespace NaNiT
{
    class UsersXML : _XmlFile
    {
        public string xUsersFile;
        public XmlDocument xUsersDoc;
        public XmlElement xUsersRoot;

        public UsersXML(string NameFile, string NameRoot = null)
        {
            xUsersFile = NameFile;
            xUsersDoc = new XmlDocument();
            Open(xUsersFile, NameRoot);
            xUsersRoot = xUsersDoc.DocumentElement;
        }
    }
}
