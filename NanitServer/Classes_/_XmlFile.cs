using System.Text.RegularExpressions;
using System.Xml;

namespace NaNiT
{
    class _XmlFile
    {
        public string xFilename;
        public XmlDocument xDoc;
        public XmlElement xRoot;

        /// <summary>
        /// Базовый XML конструктор, являющийся родительским объектом и позволяющий проводить общие операции с xml файлами
        /// -CreateDir(path); -Open(path,[root]); -Create(path); -Create(path,[root]); -Close();
        /// 
        /// </summary>

        public _XmlFile(){}

        /// <summary>
        /// Разбиваем файл на части, если он был составным
        /// Создаём дерикторию из левой части файла
        /// </summary>
        /// <param name="_name">Путь к файлу с полным именем</param>
        public void CreateDir(string _name)
        {
            string[] _shorts = Regex.Split(_name, @"(\\)");
            if (_shorts.Length > 1)
            {
                string _path = string.Join("", _shorts, 0, _shorts.Length - 1);
                System.IO.Directory.CreateDirectory(_path);
            }
        }
        /// <summary>
        /// Проверяет существование XNL файла
        /// и либо загружает его, либо создаёт
        /// </summary>
        /// <param name="_name">Путь к файлу с полным именем</param>
        /// <param name="_xroot">Рут элемент файла</param>
        public void Open(string _name, string _xroot = null)
        {
            xFilename = _name;
            if (System.IO.File.Exists(_name))
            {
                xDoc = new XmlDocument();
                xDoc.Load(_name);
            }
            else
            {
                CreateDir(_name);
                CreateXml(_name, _xroot);
            }
        }
        /// <summary>
        /// Создание нового чистого XML файла
        /// </summary>
        /// <param name="_name">Путь к файлу с полным именем</param>
        public void CreateXml(string _name)
        {
            xFilename = _name;
            xDoc = new XmlDocument();
            xDoc.RemoveAll();
            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            xDoc.Save(_name);
        }
        /// <summary>
        /// Создание нового XML файла с Рут элементом
        /// </summary>
        /// <param name="_name">Путь к файлу с полным именем</param>
        /// <param name="_xroot">Рут элемент файла</param>
        public void CreateXml(string _name, string _xroot)
        {
            xFilename = _name;
            xDoc = new XmlDocument();
            xDoc.RemoveAll();
            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            xRoot = xDoc.CreateElement(_xroot);
            xDoc.AppendChild(xRoot);
            xDoc.Save(_name);
        }
        public void Close()
        {
            xDoc.Save(xFilename);
            xDoc.RemoveAll();
            xDoc = null;
            xFilename = null;
            xRoot = null;
        }
    }

}
