using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;


namespace NaNiT
{
    class LocalGlobals
    {
        //-Forms-//
        public static FormSOptions gl_f_optionsServ = null;
        //-Connections-//
        public static ServerObject gl_c_server = null;
        //-XML-//
        public static MyXml gl_xml_users = null;
    }

    class SProgram
    {
        public static NotifyIcon notifyIcon = null;

        static void Main()
        {
            // Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            SFunctions.FirstRunOptionsLoad();
            
            // Старт таймеров и тредов
            /*Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);
            Thread ServStates = new Thread(new ThreadStart(TempServRun));
            ServStates.Start();*/

            // Упрощённая версия загрузки, где окно сразу вылетает с логом
            gl_f_optionsServ.Show();
            gl_b_isOptOpen = true;

            // Старт сервера
            FormSOptions.Start();

            _XmlFile first = new _XmlFile("last.xml");
            first.CreateXml("123", first.xRoot.Name);
            UsersXML second = new UsersXML(@"third.xml");
            second.CreateXml("123123");
            //
            Application.Run();

            
        }
    }
}