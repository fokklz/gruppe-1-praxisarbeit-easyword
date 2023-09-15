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
using static System.Collections.Specialized.BitVector32;
using System.Collections.ObjectModel;
using EasyWord.Controls;

namespace EasyWord.Pages
{
    /// <summary>
    /// Interaction logic for Lectures.xaml
    /// </summary>
    public partial class Lectures : Page
    {
public Lectures()
        {
            InitializeComponent();
            LectureWrapPanel.Children.Add(new LectureCard());
            LectureWrapPanel.Children.Add(new LectureCard());
            LectureWrapPanel.Children.Add(new LectureCard());
            LectureWrapPanel.Children.Add(new LectureCard());
            // Add a new language
            FilledComboBox.Items.Add(new ComboBoxItem
            {
                Content = "English"
            });
        }

        /// <summary>
        /// Event handler for page-size changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DynamicResizingHelper.SetMinWidths(e.NewSize.Width, ColumnStart, ColumnEnd);
            DynamicResizingHelper.SetColumns(e.NewSize.Width, LectureWrapPanel);
        }

        private void FilledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectedLecture_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllLectures_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
