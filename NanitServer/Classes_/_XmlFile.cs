using System.Text.RegularExpressions;
using System.Xml;

namespace NaNiT
{
    class _XmlFile
    {
        public string xFilename { get; set; }
        public XmlDocument xDoc;
        public XmlElement xRoot;

        /// <summary>
        /// Базовый XML конструктор, являющийся родительским объектом и позволяющий проводить общие операции с xml файлами
        /// </summary>
        /// <param name="_nameFile">Имя открываемого файла</param>
        /// <param name="_root">Рут Нода (если есть, иначе будет "Blank"</param>
        public _XmlFile(string _nameFile, params string[] _root)
        {
            xFilename = _nameFile;
            xDoc = new XmlDocument();
            if (_root.Length > 0)
                Open(xFilename, _root[0]);
            else
                Open(xFilename);
            xRoot = xDoc.DocumentElement;
        }
         
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
        /// Проверяет существование XML файла
        /// и либо загружает его, либо создаёт
        /// </summary>
        /// <param name="_name">Путь к файлу с полным именем</param>
        /// <param name="_xroot">Рут элемент файла</param>
        public void Open(string _name, params string[] _root)
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
                if (_root.Length > 0)
                    CreateXml(_name, _root[0]);
                else
                    CreateXml(_name, "BLANK");
            }
            xRoot = xDoc.DocumentElement;
        }

        /// <summary>
        /// Создание чистого XML файла с Рут элементом
        /// </summary>
        /// <param name="_name">Путь к файлу с полным именем</param>
        /// <param name="_xroot">Рут элемент файла</param>
        public void CreateXml(string _name, string _xroot = "BLANK")
        {
            xFilename = _name;
            xDoc = new XmlDocument();
            xDoc.RemoveAll();
            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            xRoot = xDoc.CreateElement(_xroot);
            xDoc.AppendChild(xRoot);
            xDoc.Save(_name);
        }

        /// <summary>
        /// Сохраняем текущий файл и зануляем переменные
        /// </summary>
        public void Close()
        {
            xDoc.Save(xFilename);
            xDoc = null;
            xFilename = null;
            xRoot = null;
        }

        /// <summary>
        /// Сохраняет активный документ
        /// </summary>
        public void Save()
        {
            if (xDoc != null && xFilename != null)
                xDoc.Save(xFilename);
        }
    }

}
