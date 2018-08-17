# TweetCounter
Simple WPF App which count the number of tweets posted with a specific hashtag.

using LinqToTwitter.

using Telegram.Bot

------------- INFO --------------------------------------
1. Create new Twitter Application via 'https://apps.twitter.com/' and copy authentication info (first 6 settings in "Properties") to app from "Keys and Access Tokens" 

tab.
2. If You want to use Telegram notification ("Twitt notify 1H") option, You must to configure last 2 option (optional).
2.1 In Telegram using 'BotFather' create new bot and copy Telegram Access Token to app.
2.2 In Telegram create new channel (public or private).
2.2.1 Login to Telegram via Web Browser 'https://web.telegram.org'and copy from addres channel ID, in example 'XXXXXXXXXX'(https://web.telegram.org/#/im?

p=cXXXXXXXXXX_123456789012).
2.2.2 Combine '-100' string and ID (e.x. -100XXXXXXXXXX) and paste into app. Now You can use option described into p.2.
2.3 Use "Update Config" option to save settings.

Buttons and other settings

- "Hashtag" textbox, write here hashtag You looking for without "#" without quotes.
- "Except" textbox, wtite here specified words You do not want into search for hashtag with "-" every word and spaces e.x. "-do -not -download" without quotes.
- "Search Language" textbox, write here language You want to search with specified hashtag, e.x. "en" without quotes.

-"ADD" button, adds data from textboxes.
-"GET" button, if You want to refresh list (it done automatically every add).
-"REMOVE" button, into datagrid below deletes hashtags from database (downloaded Tweets and Counts are not removed).

- "Twitt Get" checkbox download Tweets with specified hashtags added via first section eery 14,5 minute.
- "Twitt Get" button run manual download of Tweets with specified hashtags.
- "Count" button, compare downloaded Tweets and if have more than 2 calculations show number of Tweets in datagrid below (if in Num cell see "+" there is more than 99  

Tweets, downloaded from last full hour - but there is a limit in api calls - more on 'https://developer.twitter.com/en/docs/basics/rate-limits.html').
- "Twitt notify 1H" checkbox, described in p.2.
-"Properties" checkbox, show options to configure application.

-------------  NEXT --------------------------------------

- Add last auto-download in last minute of every hour for more performance in count




