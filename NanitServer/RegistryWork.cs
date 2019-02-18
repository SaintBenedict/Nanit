using Microsoft.Win32;
using System;

namespace NaNiT
{
    public class RegistryWork
    {
        protected internal RegistryKey SubKey;
        protected internal string sSubKey = null;
        protected internal RegistryKey regWorkLocal;
        protected internal RegistryKey regWorkNanit;

        public RegistryWork(string newSub = @"N.A.N.I.T")
        {
            regWorkLocal = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            if (newSub != @"N.A.N.I.T")
            {
                regWorkNanit = regWorkLocal.CreateSubKey(@"N.A.N.I.T");
                sSubKey = newSub;
                SubKey = regWorkNanit.CreateSubKey(newSub);
            }
            else
            {
                SubKey = regWorkLocal.CreateSubKey(@"N.A.N.I.T");
            }
        }

        public void Exit()
        {
            SubKey.Close();
            SubKey = null;
            if (sSubKey != null)
            {
                regWorkNanit.Close();
                regWorkNanit = null;
                sSubKey = null;
            }
            regWorkLocal.Close();
            regWorkLocal = null;
        }

        public string ReadString(string From, string VarTo)
        {
            string resultStr = VarTo;
            if (SubKey.GetValue(From) != null)
                resultStr = SubKey.GetValue(From).ToString();
            else
                SubKey.SetValue(From, VarTo);
            return resultStr;
        }
        public bool ReadBool(string From, bool BoolTo)
        {
            bool resultBool = BoolTo;
            if (SubKey.GetValue(From) != null)
                resultBool = SubKey.GetValue(From).Equals("true");
            else
                SubKey.SetValue(From, BoolTo.ToString().ToLower());
            return resultBool;
        }
        public int ReadInt(string From, int IntTo)
        {
            int resultInt = IntTo;
            if (SubKey.GetValue(From) != null)
                resultInt = Convert.ToInt32(SubKey.GetValue(From));
            else
                SubKey.SetValue(From, IntTo.ToString());
            return resultInt;
        }

        public void Write(string RegTo, object From)
        {
            Type TypeTo = From.GetType();
            string type = TypeTo.Name;
            switch (type)
            {
                case "String":
                    SubKey.SetValue(RegTo, From);
                    break;
                case "Boolean":
                    SubKey.SetValue(RegTo, From.Equals("true"));
                    break;
                default:
                    SubKey.SetValue(RegTo, From.ToString());
                    break;
            }
        }

    }
}