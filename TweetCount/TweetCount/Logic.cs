using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using System.Data;
using System.Collections.ObjectModel;
using TwSearch;
using Telegram.Bot;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Threading;

namespace TweetCounter
{

    class Logic
    {
        public static List<Configuration> config_list = new List<Configuration>();
        public string telegrammessageTwitterCount;
        public static int timerTwittGet = 1;
        public static int timerTwittNotify = 1;
        public static int timerTwittDelta = 1;

        DataConnection dbobject = new DataConnection();
        public static SQLiteConnection SQLconnect = new SQLiteConnection();
        MainWindow mw = (MainWindow)Application.Current.MainWindow;

        public void RemoveList(long rowid)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "REMOVE Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.RemoveFromWatchlist(rowid);
            }
        }

        public void RemoveFromWatchlist(long rowid)
        {
            mw.watchlist_list.Clear();

            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM WATCHLIST WHERE rowid='" + rowid + "'", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
                
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT distinct rowid,Hashtag,TwExcept,SearchLanguage FROM WATCHLIST", SQLconnect))
            {
                using (SQLiteDataReader r = cmd.ExecuteReader())
                {
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            mw.watchlist_list.Add(new Watchlist() { rowid = (long)r["rowid"], Hashtag = (string)r["Hashtag"], Except = (string)r["TwExcept"], SearchLanguage = (string)r["SearchLanguage"] });
                        }
                    }
                }
            }

            mw.dataGrid_WATCHLIST.ItemsSource = null;
            mw.dataGrid_WATCHLIST.ItemsSource = mw.watchlist_list;
            mw.dataGrid_WATCHLIST.Items.Refresh();

            if (SQLconnect.State == ConnectionState.Open)
            {
                SQLconnect.Close();
            }

            GetFromWatchlist();
        }

           
        public void GetFromWatchlist()
        {
            mw.watchlist_list.Clear();

            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT distinct rowid,Hashtag,TwExcept,SearchLanguage FROM WATCHLIST", SQLconnect))
            {
                using (SQLiteDataReader r = cmd.ExecuteReader())
                {
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            mw.watchlist_list.Add(new Watchlist() { rowid = (Int64)r["rowid"], Hashtag = (string)r["Hashtag"], Except = (string)r["TwExcept"], SearchLanguage = (string)r["SearchLanguage"] });
                        }
                    }
                } 
            }
            mw.dataGrid_WATCHLIST.Columns[0].Visibility = Visibility.Collapsed;
            mw.dataGrid_WATCHLIST.ItemsSource = null;
            mw.dataGrid_WATCHLIST.ItemsSource = mw.watchlist_list;
            mw.dataGrid_WATCHLIST.Items.Refresh();

            if (SQLconnect.State == ConnectionState.Open)
            {
                SQLconnect.Close();
            }
        }

        public void AddToWatchlist()
        {
            mw.watchlist_list.Clear();
            String HT = mw.WATCHLIST_HASHTAG_TB.Text;
            String Except = mw.WATCHLIST_EXCEPT_TB.Text;
            String SearchLanguage = mw.WATCHLIST_SEARCHLANG_TB.Text;

            if (Except == "")
            {
                Except = " ";
            }
            if (SearchLanguage == "")
            {
                SearchLanguage = " ";
            }

            if (HT != "")
            {
                    if (SQLconnect.State == ConnectionState.Closed)
                    {
                        SQLconnect.ConnectionString = dbobject.datalocation();
                        SQLconnect.Open();
                    }

                using (SQLiteCommand cmd = new SQLiteCommand("insert into WATCHLIST (Hashtag,TwExcept,SearchLanguage)  values (@Hashtag,@TwExcept,@SearchLanguage)", SQLconnect))
                {
                    cmd.Parameters.AddWithValue("@Hashtag", HT);
                    cmd.Parameters.AddWithValue("@TwExcept", Except);
                    cmd.Parameters.AddWithValue("@SearchLanguage", SearchLanguage);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT distinct Hashtag,TwExcept,SearchLanguage FROM WATCHLIST", SQLconnect))
                {
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null)
                        {
                            while (r.Read())
                            {
                                mw.watchlist_list.Add(new Watchlist() { Hashtag = (string)r["Hashtag"], Except = (string)r["TwExcept"], SearchLanguage = (string)r["SearchLanguage"] });
                            }
                        }
                    }
                }

                mw.dataGrid_WATCHLIST.ItemsSource = null;
                mw.dataGrid_WATCHLIST.ItemsSource = mw.watchlist_list;
                mw.dataGrid_WATCHLIST.Items.Refresh();

            }
                    mw.WATCHLIST_HASHTAG_TB.Clear();
                    mw.WATCHLIST_EXCEPT_TB.Clear();
                    mw.WATCHLIST_SEARCHLANG_TB.Clear();
                    mw.dataGrid_WATCHLIST.Items.Refresh();
        }


        public void TwitterBtn()
        {

            TwitterSearch c1 = new TwitterSearch();
            string Count = c1.TweetSearch(mw.watchlist_list);
            Count = "";
            Logs log = new Logs();
            log.LogMessageToFile("TWITTER GET DONE");
        }

        public void GetConfig(out List<Configuration>config_list)
        {
            config_list = new List<Configuration>();

            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM CONFIG", SQLconnect))
            {
                using (SQLiteDataReader r = cmd.ExecuteReader())
                {
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            config_list.Add(new Configuration() {
                                TwitterAccessToken = (string)r["TwitterAccessToken"],
                                TwitterAccessTokenSecret = (string)r["TwitterAccessTokenSecret"],
                                TwitterConsumerKey = (string)r["TwitterConsumerKey"],
                                TwitterConsumerSecret = (string)r["TwitterConsumerSecret"],
                                TwitterAcccountToDisplay = (string)r["TwitterAcccountToDisplay"],
                                TwitterUserID = (string)r["TwitterUserID"],
                                TelegramAccessToken = (string)r["TelegramAccessToken"],
                                TelegramChannel = (string)r["TelegramChannel"],
                            });
                        }
                    }
                }
            }

            if (config_list.Count != 0)
            {
                mw.TwitterAccessToken_TB.Text = config_list[0].TwitterAccessToken;
                mw.TwitterAccessTokenSecret_TB.Text = config_list[0].TwitterAccessTokenSecret;
                mw.TwitterConsumerKey_TB.Text = config_list[0].TwitterConsumerKey;
                mw.TwitterConsumerSecret_TB.Text = config_list[0].TwitterConsumerSecret;
                mw.TwitterAcccountToDisplay_TB.Text = config_list[0].TwitterAcccountToDisplay;
                mw.TwitterUserID_TB.Text = config_list[0].TwitterUserID;
                mw.TelegramAccessToken_TB.Text = config_list[0].TelegramAccessToken;
                mw.TelegramChannel_TB.Text = config_list[0].TelegramChannel;
            }




    }
        public void UpdateConfig()
        {

            string TwitterAccessToken = mw.TwitterAccessToken_TB.Text.Replace(" ", string.Empty);
            string TwitterAccessTokenSecret = mw.TwitterAccessTokenSecret_TB.Text.Replace(" ", string.Empty);
            string TwitterConsumerKey = mw.TwitterConsumerKey_TB.Text.Replace(" ", string.Empty);
            string TwitterConsumerSecret = mw.TwitterConsumerSecret_TB.Text.Replace(" ", string.Empty);
            string TwitterAcccountToDisplay = mw.TwitterAcccountToDisplay_TB.Text.Replace(" ", string.Empty);
            string TwitterUserID = mw.TwitterUserID_TB.Text.Replace(" ", string.Empty);
            string TelegramAccessToken = mw.TelegramAccessToken_TB.Text.Replace(" ", string.Empty);
            string TelegramChannel = mw.TelegramChannel_TB.Text.Replace(" ", string.Empty);


            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }

            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM CONFIG ", SQLconnect))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }



            using (SQLiteCommand cmd = new SQLiteCommand("insert into CONFIG(TwitterAccessToken, TwitterAccessTokenSecret, TwitterConsumerKey, TwitterConsumerSecret, TwitterAcccountToDisplay, TwitterUserID, TelegramAccessToken, TelegramChannel)  values(@TwitterAccessToken,@TwitterAccessTokenSecret,@TwitterConsumerKey,@TwitterConsumerSecret,@TwitterAcccountToDisplay,@TwitterUserID,@TelegramAccessToken,@TelegramChannel)", SQLconnect))
            {
                cmd.Parameters.AddWithValue("@TwitterAccessToken", TwitterAccessToken);
                cmd.Parameters.AddWithValue("@TwitterAccessTokenSecret", TwitterAccessTokenSecret);
                cmd.Parameters.AddWithValue("@TwitterConsumerKey", TwitterConsumerKey);
                cmd.Parameters.AddWithValue("@TwitterConsumerSecret", TwitterConsumerSecret);
                cmd.Parameters.AddWithValue("@TwitterAcccountToDisplay", TwitterAcccountToDisplay);
                cmd.Parameters.AddWithValue("@TwitterUserID", TwitterUserID);
                cmd.Parameters.AddWithValue("@TelegramAccessToken", TelegramAccessToken);
                cmd.Parameters.AddWithValue("@TelegramChannel", TelegramChannel);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            List<Configuration> config_list;
            GetConfig(out config_list);
        }



        public void CountBtn()
        {

            var time = DateTime.Now;
            var hour = (time.Hour);

            if (hour == 0)
            {

                hour = 23;
            }
            else
            {
                hour = hour;
            }

            TwitterCount c1 = new TwitterCount();
            string telegrammessageTwitterCount = c1.TweetCounter(mw.watchlist_list, hour);

            if (mw.TelegramAccessToken_TB.Text != "" && mw.TelegramChannel_TB.Text !="")
            {
                TwitterNotify(telegrammessageTwitterCount);
            }
          
            telegrammessageTwitterCount = "";
            Logs log = new Logs();
            log.LogMessageToFile("TWITTER COUNT DONE");
        }

        public async Task TwitterNotify(string telegrammessageTwitterCount)
        {
            if (mw.TelegramAccessToken_TB.Text != "" && mw.TelegramChannel_TB.Text != "")
            {
                String TelegramAccessToken = mw.TelegramAccessToken_TB.Text;
                String TelegramChannel = mw.TelegramChannel_TB.Text;

                if (TelegramAccessToken != "" && TelegramChannel != "")
                {
                    if (telegrammessageTwitterCount != null)
                    {
                        var botX = new TelegramBotClient(TelegramAccessToken); ;
                        var sX = await botX.SendTextMessageAsync(TelegramChannel, telegrammessageTwitterCount);
                        telegrammessageTwitterCount = null;
                    }
                    // return "Finished";
                }
            }
        }



        public void TWITT_GET_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                {
                    TwitterSearch c1 = new TwitterSearch();
                    string Count = c1.TweetSearch(mw.watchlist_list);

                    Logs log = new Logs();
                    log.LogMessageToFile("TWITTER GET auto at: ");
                }

                if (timerTwittGet == 1)
                {
                    System.Timers.Timer twGT = new System.Timers.Timer();
                    twGT.AutoReset = true;
                    twGT.Interval = 870000;
                    twGT.Elapsed += TWITT_GET_Elapsed;
                    twGT.Start();
                    timerTwittGet++;
                }
            });

        }
        public async void TWITT_NOTIFY_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //string result = await TwitterNotify();
            Application.Current.Dispatcher.Invoke(() =>
            {
                {
                    var time = DateTime.Now;
                    var hour = (time.Hour);
                    if (hour == 0)
                    {
                        hour = 23;
                    }
                    else
                    {
                        hour = hour - 1;
                    }

                    TwitterCount c1 = new TwitterCount();
                    string telegrammessageTwitterCount = c1.TweetCounter(mw.watchlist_list, hour);

                    //string result = await TwitterNotify();
                    // TwitterNotify();
                    Logs log = new Logs();
                    log.LogMessageToFile("TWITTER COUNT auto-calculated DONE");
                }

                TwitterNotify(telegrammessageTwitterCount);
                Logs logg = new Logs();
                logg.LogMessageToFile("TWITTER COUNT auto-calculated ");
                telegrammessageTwitterCount = "";

                if (timerTwittNotify == 1)
                {
                    System.Timers.Timer twNT = new System.Timers.Timer();
                    twNT.Interval = 3600000;
                    twNT.AutoReset = true;
                    twNT.Elapsed += TWITT_GET_Elapsed;
                    twNT.Start();
                    timerTwittNotify++;
                }        
            });
        }

        public async void TWITT_DELTA_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (timerTwittDelta >1)
                {
                    if (mw.TwitterAccessToken_TB.Text != "" && mw.TwitterAccessTokenSecret_TB.Text != "" && mw.TwitterConsumerKey_TB.Text != "" && mw.TwitterConsumerSecret_TB.Text != "" && mw.TwitterAcccountToDisplay_TB.Text != "" && mw.TwitterUserID_TB.Text != "")
                    {
                        string retstring = TwitterDelta(sender, e);

                        Logs logg = new Logs();
                        logg.LogMessageToFile("TWITTER DELTA DOWNLOAD SET AT " + retstring);
                    }
                }
                if (timerTwittDelta == 1)
                {
                    System.Timers.Timer twDLT = new System.Timers.Timer();
                    twDLT.Interval = 3600000;
                    twDLT.AutoReset = true;
                    twDLT.Elapsed += TWITT_DELTA_Elapsed;
                    twDLT.Start();
                    timerTwittDelta++;
                }                 
            });
        }

        private string TwitterDelta(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime time = Convert.ToDateTime(DateTime.Now);
            var HH = time.Hour;
            var MM = time.Minute;

            if (MM < 58)
            {
                SetUpTimer(new TimeSpan(Convert.ToInt32(HH), Convert.ToInt32("58"), Convert.ToInt32("00")));
            }
            else
            {
                if (HH == 23)
                {
                    HH = 0;
                    SetUpTimer(new TimeSpan(Convert.ToInt32(HH), Convert.ToInt32("58"), Convert.ToInt32("00")));
                }
                else
                {
                    HH = HH + 1;
                    SetUpTimer(new TimeSpan(Convert.ToInt32(HH), Convert.ToInt32("58"), Convert.ToInt32("00")));
                }               
            }
            string retstring = HH + ":58:00";
            return retstring;
        }


        private System.Threading.Timer timer;
        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;
            }
            this.timer = new System.Threading.Timer(async x =>
            {
               await TwDelta();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }

        public async Task TwDelta()
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                TwitterSearch SearchDelta = new TwitterSearch();
                SearchDelta.TweetSearch(mw.watchlist_list);
            });
            return;
        }


        public void GetTopTweets()
        {
            List <TopTweets> TopTweets_list = new List<TopTweets>();
            string val = mw.TOP_COMBO.Text;
            if (val == "TOP 3")
            {
                val = "3";
            }
            if (val == "TOP 5")
            {
                val = "5";
            }
            if (val == "TOP 10")
            {
                val = "10";
            }

            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }
            foreach(var element in mw.watchlist_list)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT Hashtag,SearchLanguage,ScreenNameResponse,RTcount,Text FROM TWEETS WHERE Hashtag='" + element.Hashtag + "' AND SearchLanguage ='" + element.SearchLanguage + "' ORDER BY RTcount DESC LIMIT " + val + "", SQLconnect))
                {
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null)
                        {
                            while (r.Read())
                            {
                                TopTweets_list.Add(new TopTweets() { Hashtag = (string)r["Hashtag"], SearchLanguage = (string)r["SearchLanguage"],  ScreenNameResponse = (string)r["ScreenNameResponse"], RTcount = (Int64)r["RTcount"], Text = (string)r["Text"] });
                            }
                        }
                    }
                }
            }
         
            mw.dataGrid_TOP_TWEETS.ItemsSource = null;
            mw.dataGrid_TOP_TWEETS.ItemsSource = TopTweets_list;
            mw.dataGrid_TOP_TWEETS.Items.Refresh();

            if (SQLconnect.State == ConnectionState.Open)
            {
                SQLconnect.Close();
            }
        }

        public void GetLastTweets()
        {
            List<LastTweets> LastTweets_list = new List<LastTweets>();

            DateTime time = Convert.ToDateTime(DateTime.Now.AddDays(-3));
            var Ticks = time.Ticks;

            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }
            foreach (var element in mw.watchlist_list)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT Hashtag,SearchLanguage,CreatedAt,ScreenNameResponse,RTcount,Text FROM TWEETS WHERE Ticks >='"+ Ticks + "' ORDER BY Hashtag", SQLconnect))
                {
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null)
                        {
                            while (r.Read())
                            {
                                LastTweets_list.Add(new LastTweets() { Hashtag = (string)r["Hashtag"], SearchLanguage = (string)r["SearchLanguage"], CreatedAt = (string)r["CreatedAt"], ScreenNameResponse = (string)r["ScreenNameResponse"], RTcount = (Int64)r["RTcount"], Text = (string)r["Text"] });
                            }
                        }
                    }
                }
            }

            mw.dataGrid_TWEETS_BROWSER.ItemsSource = null;
            mw.dataGrid_TWEETS_BROWSER.ItemsSource = LastTweets_list;
            mw.dataGrid_TWEETS_BROWSER.Items.Refresh();

            if (SQLconnect.State == ConnectionState.Open)
            {
                SQLconnect.Close();
            }
        }



    }
}