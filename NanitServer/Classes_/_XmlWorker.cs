using System.Xml;

namespace NaNiT
{
    class _XmlWorker : _XmlFile
    {
        public string workRoot = "users";
        public string workSingle = "user";
        public XmlNodeList xElements;

        /// <summary>
        /// XML воркер, работает с нодами документа
        /// </summary>
        /// <param name="_nameFile">Путь к файлу XML</param>
        /// <param name="_workRoot">1 - Root элемент (иначе юзерс), 2 - мейн элемент (иначе юзер)</param>
        public _XmlWorker(string _nameFile, params string[] _workRoot) :base(_nameFile, _workRoot)
        {
            if (_workRoot[0] != null) workRoot = _workRoot[0];
            if (_workRoot[1] != null) workSingle = _workRoot[1];
        }

        /// <summary>
        /// Добавляет одну основную ветку с детьми (без аттрибутов)
        /// </summary>
        /// <param name="_chilNodes">Массив детей</param>
        /// <param name="_chilValues">Массив детских значений</param>
        public void AddMain(string[] _chilNodes, string[] _chilValues)
        {
            XmlElement newElem = xDoc.CreateElement(workSingle);
            for (int i = 0; i < _chilNodes.Length; i++)
            {
                XmlElement newChild = xDoc.CreateElement(_chilNodes[i]);
                XmlText childText = xDoc.CreateTextNode(_chilValues[i]);
                newChild.AppendChild(childText);
                newElem.AppendChild(newChild);
            }
            xRoot.AppendChild(newElem);
            xDoc.Save(xFilename);
        }
    }
}
