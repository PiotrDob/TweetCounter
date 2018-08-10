using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace TweetCounter
{
    class Logs
    {


        // -------------------------- CREATE LOG FILE
        public string GetTempPath()
        {
            string path = (System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "..\\..\\")));
            return path;
            

        }

        public void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetTempPath() + "LogFile.log");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }
        // -------------------------- END CREATE LOG FILE


    }
}
