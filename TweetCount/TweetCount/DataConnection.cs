using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;
using System.Windows;

namespace TweetCounter
{

    class DataConnection
    {
        MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;

        public string datalocation()
        {
            string dir = "";

                GetDatabasePath path = new GetDatabasePath();
                string DatabaseFile = path.getDatabasePath();
               
                if (DatabaseFile.Length != 0)
                {
                    dir = DatabaseFile;
                }

            else
            {
                var filename = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "DATABASE\\PrimaryDB.mdf");
                if (!System.IO.File.Exists(filename))
                {
                    CreateDatabaseFile create = new CreateDatabaseFile();
                    create.createDB();
                }
               
                dir = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "DATABASE\\PrimaryDB.mdf");
            }
            
            string datasource = "Data Source=" + dir;

            return datasource;
        }

    }
}
