using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TweetCounter
{
    class GetDatabasePath
    {
public string getDatabasePath()
        {
            string databasePath = "";
            try
            {
                string ConfigFile = (System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "ConfigFile.txt")));
                using (StreamReader sr = new StreamReader(@ConfigFile))
                {
                    string line;
                    

                    while ((line = sr.ReadLine()) != null)
                    {
                        databasePath = line;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);

            }

            return databasePath;

        }
   

    }
}
