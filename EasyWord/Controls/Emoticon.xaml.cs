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

namespace EasyWord.Controls
{
    /// <summary>
    /// Interaction logic for Emoticon.xaml
    /// </summary>
    public partial class Emoticon : UserControl
    {
        /// <summary>
        /// Show how many Words in whitch bucket
        /// </summary>
        public float[] Counts { get; set; }

        private string _values;
        public string Values
        {
            get
            {
                return _values;        
            }
            set
            {
                Counts = value.Split(separator: ',').Select(float.Parse).ToArray();
                _values = value;
            }
        }

        
        private string _active;

        /// <summary>
        /// Change all Buckets to 0.3 Opacity, when word is true or false
        /// the bucket change the opacity
        /// </summary>
        public string Active
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

                switch (int.Parse(value))
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
        public Emoticon()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
