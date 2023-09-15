using EasyWord.Common;
using EasyWord.Data.Models;
using Microsoft.Win32;
using System;
using System.Globalization;
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

namespace EasyWord.Pages
{
    /// <summary>
    /// Interaction logic for Lections.xaml
    /// </summary>
    public partial class Lections : Page
    {
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DynamicResizingHelper.SetMinWidths(e.NewSize.Width, ColumnStart, ColumnEnd);
        }

        private void FilledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectedLection_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllLections_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
