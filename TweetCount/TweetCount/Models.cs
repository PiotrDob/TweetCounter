using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetCounter
{

    public class Tweets
    {
        public string Hashtag { get; set; }
        public string Except { get; set; }
        
    }

    public class Watchlist
    {
        public long rowid { get; set; }
        public string Hashtag { get; set; }
        public string Except { get; set; }
        public string SearchLanguage { get; set; }
    }

    public class TwitterHelpers
    {
        public string Hashtag { get; set; }

        public int twhH { get; set; }
        public int twhM { get; set; }
        public int twhS { get; set; }
        public long twhTicks { get; set; }
        public string CreatedAt { get; set; }
        public string Count { get; set; }
        public string Date { get; set; }
        public string MinuteFirst { get; set; }
        public string c { get; set; }


    }

    public class TwitterCombo
    {
        public string Hashtag { get; set; }
        public string Date { get; set; }
        public string combo { get; set; }
        public string Number { get; set; }
        public string num { get; set; }
    }



    public class Configuration
    {
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessTokenSecret { get; set; }
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }
        public string TwitterAcccountToDisplay { get; set; }
        public string TwitterUserID { get; set; }
        public string TelegramAccessToken { get; set; }
        public string TelegramChannel { get; set; }

    }
}
