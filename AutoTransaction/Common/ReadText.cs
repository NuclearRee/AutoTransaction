using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTransaction.Common
{
    public class ReadText
    {
        public static List<string> Read()
        {
            string datastr = DateTime.Today.ToString("yyyyMMdd");
            List<string> strlist = new List<string>();
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            StreamReader sr = new StreamReader(path + @"\" + datastr + " 资金股份查询.txt", Encoding.Default);
            String line;
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                i++;
                if (i > 4)
                {
                    var str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line.ToString(), "|");
                    str = str.Substring(0, str.Length - 1);
                    strlist.Add(str);

                }

                Console.WriteLine(i + " " + line.ToString());
            }
            return strlist;
        }
    }
}
