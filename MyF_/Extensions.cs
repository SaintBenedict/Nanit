using System.Threading;

namespace NaNiT
{
    public static class ThreadName
    {
        public static void Now(Thread thread, string _name)
        {
            if (thread.Name == null)
                thread.Name = _name;
        }
        public static void Current(string _name)
        {
            Thread temp = Thread.CurrentThread;
            if (temp.Name == null)
                temp.Name = _name;
        }
    }
}
