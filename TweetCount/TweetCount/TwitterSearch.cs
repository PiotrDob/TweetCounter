using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetCounter;
using System.Data.SQLite;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics;
using LinqToTwitter;

namespace TwSearch
{


    class TwitterSearch
    {

        DataConnection dbobject = new DataConnection();
        public static SQLiteConnection SQLconnect = new SQLiteConnection();
        MainWindow mw = (MainWindow)Application.Current.MainWindow;

        public string TweetSearch(List<Watchlist> watchlist_list)
        {
            Logic logic = new Logic();

            String TwitterAccessToken = mw.TwitterAccessToken_TB.Text;
            String TwitterAccessTokenSecret = mw.TwitterAccessTokenSecret_TB.Text;
            String TwitterConsumerKey = mw.TwitterConsumerKey_TB.Text;
            String TwitterConsumerSecret = mw.TwitterConsumerSecret_TB.Text;
            String TwitterAcccountToDisplay = mw.TwitterAcccountToDisplay_TB.Text;
            int TwitterUserID = Convert.ToInt32(mw.TwitterUserID_TB.Text);


            //---------------------------   DECLARATIONS

            List<String> SearchHashtags = new List<String>();
            List<TwitterHelpers> TWITTER_HELPERS = new List<TwitterHelpers>();
            List<string[]> WATCHLISTCOMBINED_HELPER_s = new List<string[]>();
            int Count = 0;



            //---------------------------   ACCESS TOKEN
            string accessToken = string.Empty;
            accessToken = TwitterAccessToken;

            string accessTokenSecret = string.Empty;
            accessTokenSecret = TwitterAccessTokenSecret;

            string consumerKey = string.Empty;
            consumerKey = TwitterConsumerKey;

            string consumerSecret = string.Empty;
            consumerSecret = TwitterConsumerSecret;

            string twitterAcccountToDisplay = string.Empty;
            twitterAcccountToDisplay = TwitterAcccountToDisplay;

            var authorizer = new SingleUserAuthorizer
            {

                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    OAuthToken = accessToken,
                    OAuthTokenSecret = accessTokenSecret,
                    ScreenName = twitterAcccountToDisplay,
                    UserID = Convert.ToUInt64(TwitterUserID)

                }
            };
            //---------------------------   ACCESS TOKEN


            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }


            foreach (var element in watchlist_list)
            {
                String Hashtag = element.Hashtag;
                String Except = element.Except;
                String SearchLanguage = element.SearchLanguage;

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT Hashtag,twH,twM,twS,Ticks FROM TWEETS WHERE Hashtag='" + Hashtag + "' ORDER BY CreatedAt DESC LIMIT 1", SQLconnect))
                {
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null)
                        {
                            while (r.Read())
                            {
                                TWITTER_HELPERS.Add(new TwitterHelpers() { Hashtag = (string)r["Hashtag"], twhH = Convert.ToInt32((string)r["twH"]), twhM = Convert.ToInt32((string)r["twM"]), twhS = Convert.ToInt32((string)r["twS"]), twhTicks = Convert.ToInt64((string)r["Ticks"]) });
                            }
                        }
                    }
                }

               // Count = 0;

                TwitterContext twitterCtx = new TwitterContext(authorizer);
                string searchTerm = Hashtag;
                string exclude = Except;
                string lang = SearchLanguage;

                if (lang.Length > 1)
                {
                    if (exclude.Length > 1)
                    {
                        searchTerm = searchTerm + " " + exclude + "lang:" + lang;
                    }
                    else
                    {
                        searchTerm = searchTerm + "lang:" + lang;
                    }

                }
                else
                {
                    if (exclude.Length > 1)
                    {
                        searchTerm = searchTerm + " " + exclude;
                    }
                    else
                    {
                        searchTerm = searchTerm;
                    }
                }
                

                Logs logs = new Logs();
                logs.LogMessageToFile("Search for the tweet " + searchTerm);


                //List<Search> searchResultsList = new List<Search>();
                var context = new TwitterContext(authorizer);
                var maxCount = 100;
                DateTime LocalDate = Convert.ToDateTime(DateTime.Now);
                var untilDate = new DateTime(LocalDate.Year, LocalDate.Month, (LocalDate.Day) + 1);
                int StartHour = LocalDate.Hour;
                int StartMinute = LocalDate.Minute;
                int StartSecond = LocalDate.Second;
                long StartTicks = LocalDate.Ticks;

                string StartT = Convert.ToString(StartTicks).Substring(0, 11);
                StartT = StartT + "0000000";
                StartTicks = Convert.ToInt64(StartT);

                string Minute = "0";
                string MinuteFirst = "0";
                string SMinute = "0";
                var tDateFirst = new DateTime();

                if (StartMinute < 10)
                {
                    SMinute = Convert.ToString(StartMinute);
                    SMinute = "0" + Convert.ToString(StartMinute);
                }    
                else
                {
                    SMinute = Convert.ToString(StartMinute);
                }

                var lastTweetInRange =
                                 (from search in context.Search
                                  where search.Type == SearchType.Search &&
                                        search.Query == searchTerm &&
                                        search.Count == maxCount &&
                                        search.Until == untilDate &&
                                        search.ResultType == ResultType.Recent &&
                                        search.IncludeEntities == true
                                  select search).LastOrDefault();

                var tweetsInRange = (from search in context.Search
                                     where search.Type == SearchType.Search &&
                                             search.Query == searchTerm &&
                                             search.Count == maxCount &&
                                             search.MaxID == lastTweetInRange.MaxID &&
                                             search.IncludeEntities == true
                                     select search).ToList();

                int i = 0;
                var time = new DateTime();
                if (tweetsInRange[0].Statuses.Count > 0)
                {
                    time = tweetsInRange[0].Statuses[i].CreatedAt;
                }

                DateTime tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                var twH = tDate.Hour;
                var twM = tDate.Minute;
                var twS = tDate.Second;
                var twTicks = tDate.Ticks;

                if (TWITTER_HELPERS.Count != 0)
                {
                    int twhH = Convert.ToInt32(TWITTER_HELPERS[0].twhH);
                    int twhM = Convert.ToInt32(TWITTER_HELPERS[0].twhM);
                    int twhS = Convert.ToInt32(TWITTER_HELPERS[0].twhS);
                    long twhTicks = Convert.ToInt64(TWITTER_HELPERS[0].twhTicks);

                    if (twH == 0)
                    { twH = 24; }

                    while (twTicks > twhTicks && (i + 1) < tweetsInRange[0].Statuses.Count)
                    //while (twH >= twhH && (twM + twS) > (twhM + twhS) && (i + 1) < tweetsInRange[0].Statuses.Count)
                    {
                        if (lastTweetInRange?.Statuses != null)
                        {
                            string ScreenNameResponse = tweetsInRange[0].Statuses[i].User.ScreenNameResponse;
                            string UserIDResponse = tweetsInRange[0].Statuses[i].User.UserIDResponse;
                            string Text = tweetsInRange[0].Statuses[i].Text;
                            string RTcount = Convert.ToString(tweetsInRange[0].Statuses[i].RetweetedStatus.RetweetCount);
                            tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                            string date = Convert.ToString(tDate);
                            date = date.Substring(0, 10);

                            time = tweetsInRange[0].Statuses[i].CreatedAt;
                            tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);

                            var timeFirst = tweetsInRange[0].Statuses[0].CreatedAt;
                            tDateFirst = TimeZoneInfo.ConvertTime(Convert.ToDateTime(timeFirst), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                            var twMFirst = tDateFirst.Minute;

                            twH = tDate.Hour;
                            twM = tDate.Minute;
                            twS = tDate.Second;
                            twTicks = tDate.Ticks;
                            //HourFromTwitterTicks = System.TimeSpan.FromTicks(Convert.ToInt64(tDate.Ticks)).Hours;
                            Count = i;

                            if (twM < 10)
                            {
                                Minute = Convert.ToString(twM);
                                Minute = "0" + Convert.ToString(twM);
                            }
                            else
                            {
                                Minute = Convert.ToString(twM);
                            }

                            if (twMFirst < 10)
                            {
                                MinuteFirst = Convert.ToString(twMFirst);
                                MinuteFirst = "0" + Convert.ToString(twMFirst);
                            }
                            else
                            {
                                MinuteFirst = Convert.ToString(twMFirst);
                            }

                            DateTime timeNext = tweetsInRange[0].Statuses[i + 1].CreatedAt;
                            DateTime tDateNext = TimeZoneInfo.ConvertTime(Convert.ToDateTime(timeNext), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                            long TicksFromTwitterTicksNext = tDateNext.Ticks;
                            //var twHNext = tDateNext.Hour;
                            //var twMNext = tDateNext.Minute;

                            if (SQLconnect.State == ConnectionState.Closed)
                            {
                                SQLconnect.Open();
                            }
                            using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETS(Hashtag, SearchLanguage, CreatedAt, twH, twM, twS, ScreenNameResponse, UserIDResponse, Text, RTcount, Ticks)  values(@Hashtag,@SearchLanguage,@CreatedAt, @twH, @twM, @twS, @ScreenNameResponse, @UserIDResponse, @Text, @RTcount, @Ticks)", SQLconnect))
                            {
                                cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                                cmd.Parameters.AddWithValue("@SearchLanguage", SearchLanguage);
                                cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                                cmd.Parameters.AddWithValue("@twH", twH);
                                cmd.Parameters.AddWithValue("@twM", Minute);
                                cmd.Parameters.AddWithValue("@twS", twS);
                                cmd.Parameters.AddWithValue("@ScreenNameResponse", ScreenNameResponse);
                                cmd.Parameters.AddWithValue("@UserIDResponse", UserIDResponse);
                                cmd.Parameters.AddWithValue("@Text", Text);
                                cmd.Parameters.AddWithValue("@RTcount", RTcount);
                                cmd.Parameters.AddWithValue("@Ticks", twTicks);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            i++;
                            twTicks = TicksFromTwitterTicksNext;
                        }
                    }
                    if (Count > 0)
                    {
                        string c = "-";
                        if (Count >= 99) { c = "+"; }
                        tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                        string date = Convert.ToString(tDate);
                        date = date.Substring(0, 10);

                        using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETSCOUNT (Hashtag,CreatedAt,Count,Date,twH,twM,MinuteFirst,c,Ticks)  values (@Hashtag,@CreatedAt,@Count,@Date,@twH,@twM,@MinuteFirst,@c,@Ticks)", SQLconnect))
                        {
                            cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                            cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                            cmd.Parameters.AddWithValue("@Count", Count + 1);
                            cmd.Parameters.AddWithValue("@Date", date);
                            cmd.Parameters.AddWithValue("@twH", twH);
                            cmd.Parameters.AddWithValue("@twM", Minute);
                            cmd.Parameters.AddWithValue("@MinuteFirst", MinuteFirst);
                            cmd.Parameters.AddWithValue("@c", c);
                            cmd.Parameters.AddWithValue("@Ticks", twTicks);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    else
                    {
                        string c = "-";
                        if (Count >= 99) { c = "+"; }
                        tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                        string date = Convert.ToString(tDate);
                        date = date.Substring(0, 10);

                        using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETSCOUNT (Hashtag,CreatedAt,Count,Date,twH,twM,MinuteFirst,c,Ticks)  values (@Hashtag,@CreatedAt,@Count,@Date,@twH,@twM,@MinuteFirst,@c,@Ticks)", SQLconnect))
                        {
                            cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                            cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                            cmd.Parameters.AddWithValue("@Count", 0);
                            cmd.Parameters.AddWithValue("@Date", date);
                            cmd.Parameters.AddWithValue("@twH", StartHour);
                            cmd.Parameters.AddWithValue("@twM", SMinute);
                            cmd.Parameters.AddWithValue("@MinuteFirst", SMinute);
                            cmd.Parameters.AddWithValue("@c", c);
                            cmd.Parameters.AddWithValue("@Ticks", twhTicks);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                else if (TWITTER_HELPERS.Count == 0)
                {
                    int HourFromTwitterTicks = System.TimeSpan.FromTicks(twTicks).Hours;
                    int HourFromStartTicks = System.TimeSpan.FromTicks(StartTicks).Hours;

                    while ((HourFromTwitterTicks >= HourFromStartTicks) && ((i + 1) < tweetsInRange[0].Statuses.Count))
                    {
                        if (lastTweetInRange?.Statuses != null)
                        {
                            string ScreenNameResponse = tweetsInRange[0].Statuses[i].User.ScreenNameResponse;
                            string UserIDResponse = tweetsInRange[0].Statuses[i].User.UserIDResponse;
                            string Text = tweetsInRange[0].Statuses[i].Text;
                            string RTcount = Convert.ToString(tweetsInRange[0].Statuses[i].RetweetedStatus.RetweetCount);
                            //DateTime CreatedAt = tweetsInRange[0].Statuses[i].CreatedAt;
                            tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                            string date = Convert.ToString(tDate);
                            date = date.Substring(0, 10);
                            time = tweetsInRange[0].Statuses[i].CreatedAt;
                            //tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);

                            var timeFirst = tweetsInRange[0].Statuses[0].CreatedAt;
                            tDateFirst = TimeZoneInfo.ConvertTime(Convert.ToDateTime(timeFirst), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                            var twMFirst = tDateFirst.Minute;
                            if (twMFirst < 10)
                            {
                                MinuteFirst = Convert.ToString(twMFirst);
                                MinuteFirst = "0" + Convert.ToString(twMFirst);
                            }

                            else
                            {
                                MinuteFirst = Convert.ToString(twMFirst);
                            }

                            twH = tDate.Hour;
                            twM = tDate.Minute;
                            twS = tDate.Second;
                            twTicks = tDate.Ticks;
                            HourFromTwitterTicks = System.TimeSpan.FromTicks(Convert.ToInt64(tDate.Ticks)).Hours;
                            Count = i + 1;

                            if (twM < 10)
                            {
                                Minute = Convert.ToString(twM);
                                Minute = "0" + Convert.ToString(twM);
                            }

                            else
                            {
                                Minute = Convert.ToString(twM);
                            }

                            DateTime timeNext = tweetsInRange[0].Statuses[i + 1].CreatedAt;
                            DateTime tDateNext = TimeZoneInfo.ConvertTime(Convert.ToDateTime(timeNext), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                            int HourFromTwitterTicksNext = System.TimeSpan.FromTicks(Convert.ToInt64(tDateNext.Ticks)).Hours;
                            //var twHNext = tDateNext.Hour;
                            //var twMNext = tDateNext.Minute;

                            if (SQLconnect.State == ConnectionState.Closed)
                            {
                                SQLconnect.Open();
                            }
                            using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETS(Hashtag, SearchLanguage, CreatedAt, twH, twM, twS, ScreenNameResponse, UserIDResponse, Text, RTcount, Ticks)  values(@Hashtag,@SearchLanguage,@CreatedAt, @twH, @twM, @twS, @ScreenNameResponse, @UserIDResponse, @Text, @RTcount, @Ticks)", SQLconnect))
                            {
                                cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                                cmd.Parameters.AddWithValue("@SearchLanguage", SearchLanguage);
                                cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                                cmd.Parameters.AddWithValue("@twH", twH);
                                cmd.Parameters.AddWithValue("@twM", Minute);
                                cmd.Parameters.AddWithValue("@twS", twS);
                                cmd.Parameters.AddWithValue("@ScreenNameResponse", ScreenNameResponse);
                                cmd.Parameters.AddWithValue("@UserIDResponse", UserIDResponse);
                                cmd.Parameters.AddWithValue("@Text", Text);
                                cmd.Parameters.AddWithValue("@RTcount", RTcount);
                                cmd.Parameters.AddWithValue("@Ticks", twTicks);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            i++;
                            HourFromTwitterTicks = HourFromTwitterTicksNext;
                        }
                    }
                    if (Count > 0)
                    {
                        string c = "-";
                        if (Count >= 99) { c = "+"; }
                        tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                        string date = Convert.ToString(tDate);
                        date = date.Substring(0, 10);

                        using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETSCOUNT (Hashtag,CreatedAt,Count,Date,twH,twM,MinuteFirst,c,Ticks)  values (@Hashtag,@CreatedAt,@Count,@Date,@twH,@twM,@MinuteFirst,@c,@Ticks)", SQLconnect))
                        {
                            cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                            cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                            cmd.Parameters.AddWithValue("@Count", Count); // +1
                            cmd.Parameters.AddWithValue("@Date", date);
                            cmd.Parameters.AddWithValue("@twH", twH);
                            cmd.Parameters.AddWithValue("@twM", Minute);
                            cmd.Parameters.AddWithValue("@MinuteFirst", MinuteFirst);
                            cmd.Parameters.AddWithValue("@c", c);
                            cmd.Parameters.AddWithValue("@Ticks", twTicks);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    else
                    {
                        string c = "-";
                        if (Count >= 99) { c = "+"; }
                        tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                        string date = Convert.ToString(tDate);
                        date = date.Substring(0, 10);

                        using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETSCOUNT (Hashtag,CreatedAt,Count,Date,twH,twM,MinuteFirst,c,Ticks)  values (@Hashtag,@CreatedAt,@Count,@Date,@twH,@twM,@MinuteFirst,@c,@Ticks)", SQLconnect))
                        {
                            cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                            cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                            cmd.Parameters.AddWithValue("@Count", 0);
                            cmd.Parameters.AddWithValue("@Date", date);
                            cmd.Parameters.AddWithValue("@twH", StartHour);
                            cmd.Parameters.AddWithValue("@twM", SMinute);
                            cmd.Parameters.AddWithValue("@MinuteFirst", SMinute);
                            cmd.Parameters.AddWithValue("@c", c);
                            cmd.Parameters.AddWithValue("@Ticks", StartTicks);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                else
                {
                   // Console.WriteLine("No entries found.");
                    if (Count == 0)
                    {
                        string c = "-";
                        if (Count >= 99) { c = "+"; }
                        tDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(time), TimeZoneInfo.Utc, TimeZoneInfo.Local);
                        string date = Convert.ToString(tDate);
                        date = date.Substring(0, 10);

                        using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETSCOUNT (Hashtag,CreatedAt,Count,Date,twH,twM,MinuteFirst,c,Ticks)  values (@Hashtag,@CreatedAt,@Count,@Date,@twH,@twM,@MinuteFirst,@c,@Ticks)", SQLconnect))
                        {
                            cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                            cmd.Parameters.AddWithValue("@CreatedAt", tDate);
                            cmd.Parameters.AddWithValue("@Count", 0);
                            cmd.Parameters.AddWithValue("@Date", date);
                            cmd.Parameters.AddWithValue("@twH", StartHour);
                            cmd.Parameters.AddWithValue("@twM", SMinute);
                            cmd.Parameters.AddWithValue("@MinuteFirst", SMinute);
                            cmd.Parameters.AddWithValue("@c", c);
                            cmd.Parameters.AddWithValue("@Ticks", StartTicks);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                TWITTER_HELPERS.Clear();
            }
            SQLconnect.Close();
            return Count.ToString();
        }
    }




    class TwitterCount
    {
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public static SQLiteConnection SQLconnect = new SQLiteConnection();
        DataConnection dbobject = new DataConnection();
        public static String telegrammessageTwitterCount;
        public static String Date;
        public static String combo;
        public static String num;


        //TODO
        public string TweetCounter(List<Watchlist> watchlist_list, int hour)
        {
            //---------------------------   DECLARATIONS
            telegrammessageTwitterCount = null;
            List<String> SearchHashtags = new List<String>();
            List<TwitterHelpers> TWITTER_HELPERS = new List<TwitterHelpers>();
            List<TwitterCombo> TWITTER_COMBO = new List<TwitterCombo>();
            List<string[]> WATCHLISTCOMBINED_HELPER_s = new List<string[]>();
            List<string> COUNTHELPER = new List<string>();
            int Number = 0;
            TWITTER_COMBO.Clear();

            if (SQLconnect.State == ConnectionState.Closed)
            {
                SQLconnect.ConnectionString = dbobject.datalocation();
                SQLconnect.Open();
            }
 
            var time = DateTime.Now;
            string date = Convert.ToString(time);

            if (hour == 23)
            {
                DateTime yesterday = DateTime.Now.Date.AddDays(-1);
                String y = Convert.ToString(yesterday);
                date = y.Substring(0, 10);
            }
            else
            {
                date = date.Substring(0, 10);
            }

            int b = 1;
            int countt = 0;

            foreach (var element in watchlist_list)
            {
                String Hashtag = element.Hashtag;

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT Hashtag,CreatedAt,Count,Date,twH,twM,MinuteFirst,c FROM TWEETSCOUNT WHERE Hashtag='" + Hashtag + "' AND twH = '" + hour + "' AND Date='" + date + "' ORDER BY twM DESC", SQLconnect))
                {
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null)
                        {
                            while (r.Read())
                            {
                                TWITTER_HELPERS.Add(new TwitterHelpers() { Hashtag = (string)r["Hashtag"], CreatedAt = (string)r["CreatedAt"], Count = (string)r["Count"], Date = (string)r["Date"], twhH = Convert.ToInt32((string)r["twH"]), twhM = Convert.ToInt32((string)r["twM"]), MinuteFirst = (string)r["MinuteFirst"], c = (string)r["c"] });
                            }
                        }
                    }
                }

                if (TWITTER_HELPERS.Count >= 2)
                {
                    countt = TWITTER_HELPERS.Count - 1;
                    int c = 0;
                    num = "";


                    string HT = TWITTER_HELPERS[0].Hashtag;
                    string CreatedAt = TWITTER_HELPERS[0].CreatedAt;
                    Date = TWITTER_HELPERS[0].Date;
                    string twH = (TWITTER_HELPERS[0].twhH).ToString();
                    string MinuteFirst = TWITTER_HELPERS[0].MinuteFirst;

                    for (int i = 0; i <= countt; i++)
                    {
                        c = Convert.ToInt32(TWITTER_HELPERS[i].Count);
                        Number += c;
                        num = TWITTER_HELPERS[i].c;

                        if (num == "+")
                        {
                            num = "+";
                        }
                        else { num = "-"; }
                    }

                    string twM1 = (TWITTER_HELPERS[countt].twhM).ToString();
                    if (twM1.Length == 1)
                    {
                        twM1 = "0" + twM1;
                    }
                    string twM2 = MinuteFirst;

                    combo = twH + ":" + twM1 + " - " + twH + ":" + twM2;

                    using (SQLiteCommand cmd = new SQLiteCommand("insert into TWEETSCOMBO (Hashtag,Date,Combo,Count,c)  values (@Hashtag,@Date,@Combo,@Count,@c)", SQLconnect))
                    {
                        cmd.Parameters.AddWithValue("@Hashtag", Hashtag);
                        cmd.Parameters.AddWithValue("@Date", Date);
                        cmd.Parameters.AddWithValue("@Combo", combo);
                        cmd.Parameters.AddWithValue("@Count", Number);
                        cmd.Parameters.AddWithValue("@c", num);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    TWITTER_COMBO.Add(new TwitterCombo() { Hashtag = Hashtag, Date = Date, combo = combo, Number = Number.ToString(), num = num });
                }
                else { }

                if (countt > 0)
                {
                    while (b == 1)
                    {
                        telegrammessageTwitterCount += "!!!  No. of TWEETS !!!" + "\n" + Date + "\n";
                        b++;
                    }

                    telegrammessageTwitterCount += Hashtag + " | " + combo + " | " + Number + " || " + num + "\n";
                }

                Number = 0;
                TWITTER_HELPERS.Clear();
                Logs log = new Logs();
                log.LogMessageToFile("Calculate Tweets about: " + Hashtag);
            }


            mw.dataGrid_COMBO.ItemsSource = null;
            mw.dataGrid_COMBO.ItemsSource = TWITTER_COMBO;
            mw.dataGrid_COMBO.Items.Refresh();

           

            if (SQLconnect.State == ConnectionState.Open)
            {
                SQLconnect.Close();
            }

            return telegrammessageTwitterCount;
        }
    }
}
