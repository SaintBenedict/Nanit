using Newtonsoft.Json;
using System;
using System.IO;
using NaNiT.Functions;

namespace NaNiT.Permissions
{
    public class User
    {
        public string name;
        public string uuid;
        public int lastOnline = 0;

        public User(string name, string uuid, int lastOnline)
        {
            this.name = name;
            this.uuid = uuid;
            this.lastOnline = lastOnline;
        }
    }

    class Users
    {
        internal static string UsersPath { get { return Path.Combine(MainProgram.SavePath, "players"); } }

        public static void SetupUsers()
        {
            if (!Directory.Exists(UsersPath)) Directory.CreateDirectory(UsersPath);
        }

        public static User GetUser(string name, string uuid)
        {
            if (File.Exists(Path.Combine(UsersPath, uuid + ".json")))
            {
                return Read(Path.Combine(UsersPath, uuid + ".json"), new string[] { name, uuid });
            }
            else
            {
                User user = new User(name, uuid, 0);
                Write(Path.Combine(UsersPath, uuid + ".json"), user);

                return user;
            }
        }

        public static void SaveUser(UserActiveInfo player)
        {
            try
            {
                User user = new User(player.name, player.uuid, Function.getTimestamp());
                Write(Path.Combine(UsersPath, player.uuid + ".json"), user);
            }
            catch (Exception e)
            {
                MainProgram.logException("Unable to save player data file for " + player.name + ": " + e.StackTrace);
            }
        }

        static User Read(string path, string[] data)
        {

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                User file = Read(fs, data);
                MainProgram.logInfo("Loaded persistant user storage for " + file.name);
                return file;
            }
        }

        static User Read(Stream stream, string[] data)
        {
            try
            {
                using (var sr = new StreamReader(stream))
                {
                    return JsonConvert.DeserializeObject<User>(sr.ReadToEnd());
                }
            }
            catch (Exception)
            {
                MainProgram.logException("Persistant user storage for " + data[0] + " is corrupt - Creating with default values");
                return new User(data[0], data[1], MainProgram.defaultGroup, false, true, Function.getTimestamp());
            }
        }

        static void Write(string path, User user)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs, user);
            }
        }

        static void Write(Stream stream, User user)
        {
            var str = JsonConvert.SerializeObject(user, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }
    }
}
