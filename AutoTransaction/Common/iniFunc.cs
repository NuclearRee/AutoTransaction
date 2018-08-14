using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoTransaction.Common
{
    public class IniFunc
    {
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileSection(string section, byte[] buffer, int nSize, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        public static int GetInt(string section, string key, int def, string fileName)
        {
            return IniFunc.GetPrivateProfileInt(section, key, def, fileName);
        }

        public static string GetString(string section, string key, string def, string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder(102400);
            IniFunc.GetPrivateProfileString(section, key, def, stringBuilder, 102400, fileName);
            return stringBuilder.ToString();
        }

        public static string GetString(string section, string key, string def, string fileName, int size)
        {
            StringBuilder stringBuilder = new StringBuilder();
            IniFunc.GetPrivateProfileString(section, key, def, stringBuilder, size, fileName);
            return stringBuilder.ToString();
        }

        public static void WriteInt(string section, string key, int iVal, string fileName)
        {
            IniFunc.WritePrivateProfileString(section, key, iVal.ToString(), fileName);
        }

        public static void WriteString(string section, string key, string strVal, string fileName)
        {
            IniFunc.WritePrivateProfileString(section, key, strVal, fileName);
        }

        public static void DelKey(string section, string key, string fileName)
        {
            IniFunc.WritePrivateProfileString(section, key, null, fileName);
        }

        public static void DelSection(string section, string fileName)
        {
            IniFunc.WritePrivateProfileString(section, null, null, fileName);
        }

        public static List<string> ReadSections(string filePath)
        {
            byte[] array = new byte[65535];
            int privateProfileSectionNamesA = IniFunc.GetPrivateProfileSectionNamesA(array, array.GetUpperBound(0), filePath);
            List<string> list = new List<string>();
            if (privateProfileSectionNamesA > 0)
            {
                int num = 0;
                for (int i = 0; i < privateProfileSectionNamesA; i++)
                {
                    if (array[i] == 0)
                    {
                        string text = Encoding.Default.GetString(array, num, i - num).Trim();
                        num = i + 1;
                        if (text != "")
                        {
                            list.Add(text);
                        }
                    }
                }
            }
            return list;
        }

        public static List<string> ReadKeyValues(string section, string filePath)
        {
            byte[] array = new byte[32767];
            List<string> list = new List<string>();
            int privateProfileSection = IniFunc.GetPrivateProfileSection(section, array, array.GetUpperBound(0), filePath);
            int num = 0;
            for (int i = 0; i < privateProfileSection; i++)
            {
                if (array[i] == 0)
                {
                    string text = Encoding.Default.GetString(array, num, i - num).Trim();
                    num = i + 1;
                    if (text.Length > 0)
                    {
                        list.Add(text);
                    }
                }
            }
            return list;
        }
    }
}
