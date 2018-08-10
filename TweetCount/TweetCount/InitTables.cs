using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;



namespace TweetCounter
{


    class CreateDatabaseFile
    {
        DataConnection dbobject = new DataConnection();
        public static SQLiteConnection SQLconnect = new SQLiteConnection();


        public void createDB()
        {
            var filename = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "DATABASE\\PrimaryDB.mdf");
            System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\DATABASE");
            SQLiteConnection.CreateFile(filename);

                InitTables init = new InitTables();
                init.createTables(filename);
    }

}
    class InitTables
    {
        DataConnection dbobject = new DataConnection();
        public static SQLiteConnection SQLconnect = new SQLiteConnection();

        public void createTables(string filename)
        {
            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = "Data Source="+ filename;
                SQLconnect.Open();
            }
            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS CONFIG (TwitterAccessToken TEXT, TwitterAccessTokenSecret TEXT, TwitterConsumerKey TEXT, TwitterConsumerSecret TEXT, TwitterAcccountToDisplay TEXT, TwitterUserID TEXT, TelegramAccessToken TEXT, TelegramChannel TEXT)", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS WATCHLIST (Hashtag TEXT,TwExcept TEXT ,SearchLanguage TEXT)", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS TWEETS (Hashtag TEXT, CreatedAt TEXT, twH TEXT, twM TEXT, twS TEXT, ScreenNameResponse TEXT, UserIDResponse TEXT, Text TEXT, RTcount INTEGER, Ticks TEXT )", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS TWEETSCOMBO (Hashtag TEXT, Date TEXT, Combo TEXT, Count TEXT, c TEXT)", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS TWEETSCOUNT (Hashtag TEXT, CreatedAt TEXT, Count TEXT, Date TEXT, twH TEXT, twM TEXT, MinuteFirst TEXT, c TEXT, Ticks TEXT )", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            if (SQLconnect.State == ConnectionState.Open)
            {
                SQLconnect.Close();
            }

        }
    }
}
