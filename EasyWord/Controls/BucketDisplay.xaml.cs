using EasyWord.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace EasyWord.Controls
{
    /// <summary>
    /// Interaction logic for Emoticon.xaml
    /// </summary>
    public partial class BucketDisplay : UserControl, INotifyPropertyChanged
    {
        private int _active;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Change all Buckets to 0.3 Opacity, when word is true or false
        /// the bucket change the opacity
        /// </summary>
        public int Active
        {
            get
            {
                return _active;
            }
            set
            {
                BucketDisplay2.Opacity = 0.3;
                BucketDisplay3.Opacity = 0.3;
                BucketDisplay4.Opacity = 0.3;
                BucketDisplay5.Opacity = 0.3;

                switch (value)
                {
                    case 2:
                        BucketDisplay2.Opacity = 0.8;
                        break;
                    case 4:
                        BucketDisplay4.Opacity = 0.8;
                        break;
                    case 5:
                        BucketDisplay5.Opacity = 0.8;
                        break;
                    default:
                        BucketDisplay3.Opacity = 0.8;
                        break;
                }
                _active = value;
            }
        }
        public BucketDisplay()
        {
            InitializeComponent();
            DataContext = this;
            App.SessionUpdated += App_SessionUpdated;
        }

        private void App_SessionUpdated(object? sender, EventArgs e)
        {
            if (App.Session == null) return;
            App.Session.Next += Session_Next;
            Session_Next(null, new Common.SessionNextEventArgs(new Word()));
        }

        private void Session_Next(object? sender, Common.SessionNextEventArgs e)
        {
            if (App.Session == null) return;
            int[] buckets = App.Session.Buckets;
            BucketCount1.Text = $"{buckets[0]}";
            BucketCount2.Text = $"{buckets[1]}";
            BucketCount3.Text = $"{buckets[2]}";
            BucketCount4.Text = $"{buckets[3]}";
            BucketCount5.Text = $"{buckets[4]}";

            Word? word = App.Session.GetNextWord();
            if (word == null) return;
            Active = word.Bucket;
        }
    }
}
