using EasyWord.Common;
using EasyWord.Data.Models;
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
    /// Interaction logic for LectureCard.xaml
    /// </summary>
    public partial class LectureCard : UserControl
    {
        public string Lecture { get; set; } = "Standard";
        public int WordCount { get; set; } = 0;

        public LectureCard()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
