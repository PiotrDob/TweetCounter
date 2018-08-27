using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Data.SQLite;
using System.Data;
using System.Collections.ObjectModel;
using Telegram.Bot;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TweetCounter
{
    public partial class MainWindow : Window
    {
        public List<Watchlist> watchlist_list = new List<Watchlist>();

        public MainWindow()
        {
            InitializeComponent();
            WATCHLIST_RB.IsChecked = true;
            Logic logic = new Logic();
            List<Configuration> config_list;
            logic.GetConfig(out config_list);
            TOP_COMBO.Text = "TOP 3";
            CONFIG_STP.Visibility = Visibility.Hidden;
            TW_BROWSER_LBL.Visibility = Visibility.Hidden;
            dataGrid_TWEETS_BROWSER.Visibility = Visibility.Hidden;
            LOAD_TWBROWSER_BTN.Visibility = Visibility.Hidden;

            this.TWEET_DELTA();
        }
        
        private void REMOVE_LIST_Click(object sender, RoutedEventArgs e)
        {

            object rowid = ((System.Windows.Controls.Button)sender).CommandParameter;
            if (rowid != null)
            {
                long rowidtoDel = Convert.ToInt64(rowid);
                Logic logic = new Logic();
                logic.RemoveList(rowidtoDel);

            }
        }

        public void GET_WATCHLIST_BTN_Click(object sender, RoutedEventArgs e)
        {
            Logic logic = new Logic();
            logic.GetFromWatchlist();
        }


        private void ADD_WATCHLIST_BTN_Click(object sender, RoutedEventArgs e)
        {
            Logic logic = new Logic();
            logic.AddToWatchlist();
            
            }

        private void WATCHLIST_RB_Checked(object sender, RoutedEventArgs e)
        {
            GET_WATCHLIST_BTN_Click(sender, e);

        }

        private void TWITTER_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (TwitterAccessToken_TB.Text != "" && TwitterAccessTokenSecret_TB.Text != "" && TwitterConsumerKey_TB.Text != "" && TwitterConsumerSecret_TB.Text != "" && TwitterAcccountToDisplay_TB.Text != "" && TwitterUserID_TB.Text != "")
            {
                Logic search = new Logic();
                search.TwitterBtn();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("One or more property is missing. Go to 'Properties'.");
            }


        }

        private void COUNT_BTN_Click(object sender, RoutedEventArgs e)
        {
            Logic count = new Logic();
            count.CountBtn();
        }



        private void TWITT_GET_CB_Checked(object sender, RoutedEventArgs e)
        {
            if (TWITT_GET_CB.IsChecked == true)
            {
                if (TwitterAccessToken_TB.Text != "" && TwitterAccessTokenSecret_TB.Text != "" && TwitterConsumerKey_TB.Text != "" && TwitterConsumerSecret_TB.Text != "" && TwitterAcccountToDisplay_TB.Text != "" && TwitterUserID_TB.Text != "")
                {
                    Logic logic = new Logic();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.AutoReset = false;
                    timer.Interval = 3600;
                    timer.Elapsed += logic.TWITT_GET_Elapsed;
                    timer.Start();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("One or more property is missing. Go to 'Properties'.");
                    TWITT_GET_CB.IsChecked = false;
                }
            }
            if (TWITT_GET_CB.IsChecked == false)
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.AutoReset = false;
                timer.Enabled = false;
                timer.Stop();
                return;
            }
        }
        private void TWITT_GET_CB_Changed(object sender, RoutedEventArgs e)
        {

            if (TWITT_GET_CB.IsChecked == true)
            {

                if (TwitterAccessToken_TB.Text != "" && TwitterAccessTokenSecret_TB.Text != "" && TwitterConsumerKey_TB.Text != "" && TwitterConsumerSecret_TB.Text != "" && TwitterAcccountToDisplay_TB.Text != "" && TwitterUserID_TB.Text != "")
                {
               Logic logic = new Logic();
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.AutoReset = false;
                timer.Interval = 3600;
                timer.Elapsed += logic.TWITT_GET_Elapsed;
                timer.Start();
                }
                else
                 {
                 System.Windows.Forms.MessageBox.Show("One or more property is missing. Go to 'Properties'.");
                 TWITT_GET_CB.IsChecked = false;
                 }
        }
            if (TWITT_GET_CB.IsChecked == false)
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.AutoReset = false;
                timer.Enabled = false;
                timer.Stop();
                return;
            }
        }

        private void TWITT_NOTIFY_CB_Checked(object sender, RoutedEventArgs e)
        {
            if (TWITT_NOTIFY_CB.IsChecked == true)
            {
                if (TelegramAccessToken_TB.Text != "" && TelegramChannel_TB.Text != "")
                {
                    Logic logic = new Logic();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.AutoReset = false;
                    timer.Interval = 3600;
                    timer.Elapsed += logic.TWITT_NOTIFY_Elapsed;
                    timer.Start();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("One or more property is missing. Go to 'Properties'.");
                    TWITT_NOTIFY_CB.IsChecked = false;
                }
            }
            if (TWITT_NOTIFY_CB.IsChecked == false)
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.AutoReset = false;
                timer.Enabled = false;
                timer.Stop();
                return;
            }
        }

        private void TWITT_NOTIFY_CB_Changed(object sender, RoutedEventArgs e)
        {

            if (TWITT_NOTIFY_CB.IsChecked == true)
            {
                if (TelegramAccessToken_TB.Text != "" && TelegramChannel_TB.Text != "")
                {
                    Logic logic = new Logic();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.AutoReset = false;
                    timer.Interval = 3600;
                    timer.Elapsed += logic.TWITT_NOTIFY_Elapsed;
                    timer.Start();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("One or more property is missing. Go to 'Properties'.");
                    TWITT_NOTIFY_CB.IsChecked = false;
                }
                if (TWITT_NOTIFY_CB.IsChecked == false)
                {
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.AutoReset = false;
                    timer.Enabled = false;
                    timer.Stop();
                    return;
                }
            }
        }


        private void TWEET_DELTA()
        {
                    Logic logic = new Logic();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.AutoReset = false;
                    timer.Interval = 3600;
                    timer.Elapsed += logic.TWITT_DELTA_Elapsed;
                    timer.Start();    
        }


        private void UPDATE_CONFIG_BTN_Click(object sender, RoutedEventArgs e)
        {
            Logic logic = new Logic();
            logic.UpdateConfig();
        }

        private void PROPERTIES_CB_Checked(object sender, RoutedEventArgs e)
        {
            if (PROPERTIES_CB.IsChecked == true)
            {
                if (TW_BROWSER_CB.IsChecked == true)
                {
                    CONFIG_STP.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Width = 1220;
                    CONFIG_STP.Visibility = Visibility.Visible;
                }
               
            }
            if (PROPERTIES_CB.IsChecked == false)
            {
                if (TW_BROWSER_CB.IsChecked == true)
                {
                    CONFIG_STP.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Width = 776;
                    CONFIG_STP.Visibility = Visibility.Hidden;
                }

            }

        }


        private void UPDATE_TOP_BTN_Click(object sender, RoutedEventArgs e)
        {
            Logic TopTweets = new Logic();
            TopTweets.GetTopTweets();
        }

        private void LOAD_TWBROWSER_BTN_Click(object sender, RoutedEventArgs e)
        {
            Logic TopTweets = new Logic();
            TopTweets.GetLastTweets();
        }

        private void TW_BROWSER_CB_Checked(object sender, RoutedEventArgs e)
        {

            if (TW_BROWSER_CB.IsChecked == true)
            {
                this.Width = 1810;
                TW_BROWSER_LBL.Visibility = Visibility.Visible;
                dataGrid_TWEETS_BROWSER.Visibility = Visibility.Visible;
                LOAD_TWBROWSER_BTN.Visibility = Visibility.Visible;
            }
            if (TW_BROWSER_CB.IsChecked == false)
                if (PROPERTIES_CB.IsChecked == false)
                {
                    this.Width = 776;
                    TW_BROWSER_LBL.Visibility = Visibility.Hidden;
                    dataGrid_TWEETS_BROWSER.Visibility = Visibility.Hidden;
                    LOAD_TWBROWSER_BTN.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Width = 1220;
                    TW_BROWSER_LBL.Visibility = Visibility.Hidden;
                    dataGrid_TWEETS_BROWSER.Visibility = Visibility.Hidden;
                    LOAD_TWBROWSER_BTN.Visibility = Visibility.Hidden;
                
                }
            }
        }
    }





    

