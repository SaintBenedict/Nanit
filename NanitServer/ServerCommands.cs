
namespace NaNiT
{
    class ServerCommands
    {
        public static string CheckCommand(string input)
        {
            string result = input;
            switch (input)
            {
                default:
                    Globals.MessageText = input;
                    Globals.MessageIn++;
                    break;


            }
            return result;
        }
    }
}
