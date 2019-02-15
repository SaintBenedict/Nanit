using System.Threading;
using System.Windows.Forms;
using System.Xml;
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
        public static XmlUser gl_xml = null;
    }

    class ServerProgram
    {
        static void Main()
        {
            // Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            _Loading _ServerLoad = new _Loading();
                        
            // Старт таймеров и тредов
            /*Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);
            Thread ServStates = new Thread(new ThreadStart(TempServRun));
            ServStates.Start();*/

            

            // Старт сервера
            FormSOptions.Start();
            
            
            //
            Application.Run();

            
        }
    }
}