using System.Xml;

namespace NaNiT
{
    class UsersXML : _XmlFile
    {
        private const string NameRoot = "users";

        public UsersXML(string _nameFile, params string[] _root) :base(_nameFile, NameRoot)
        {
            /*xFilename = _nameFile;
            xDoc = new XmlDocument();
            Open(xFilename, NameRoot);
            xRoot = xDoc.DocumentElement;*/
        }
    }
}
