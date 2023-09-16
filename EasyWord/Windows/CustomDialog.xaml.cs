using EasyWord.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace EasyWord.Windows
{
    /// <summary>
    /// Interaktionslogik für CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : Window
    {
        public IEnumerable<Word> Data
        {
            get
            {
                return (IEnumerable<Word>)DuplicatedWords.ItemsSource;
            }
            set
            {
                DuplicatedWords.ItemsSource = value;
            }
        }

        public CustomDialog()
        {
            InitializeComponent();
        }

        private void BtnSubmit(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
