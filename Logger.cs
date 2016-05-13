using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Integration_Test
{
    public class Logger
    {
        private string folder { get; set; }

        public Logger()
        {
            folder = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())),"LogTests");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public void WriteToFile(string text, string filename)
        {
            string logFileName = Path.Combine(folder, filename + "_" + DateTime.Now.ToString("ddMMyyyy_hh_mm_ss") + ".sql");
            File.WriteAllText(logFileName, text.Substring(0, text.Length), Encoding.Default);
        }

        public static string GetInlineParamData(Object objRet)
        {
            var dadosRet = new StringBuilder();

            foreach (PropertyInfo property in objRet.GetType().GetProperties())
            {
                object value = property.GetValue(objRet, null);
                dadosRet.AppendFormat("{0} = {1} | ", property.Name, value);
            }

            return dadosRet.ToString();
        }
    }
}
