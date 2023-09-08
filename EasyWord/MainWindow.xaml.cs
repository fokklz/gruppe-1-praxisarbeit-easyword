using EasyWord.Common;
using Microsoft.Win32;
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

namespace EasyWord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateView();
        }

        private void UpdateView()
        {
            title.Content = App.Config.Words.Title ?? "Bitte csv Datei importieren";
            WordOutput.Content = App.Config.Words.GetNextWord().Question;
            WordInput.Text = "";
        }


        /// <summary>
        /// built in hook
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.SaveSettings();
        }

        /// <summary>
        /// Event handler for the "btnCsvImport" button click
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">Additional event data</param>
        private void btnCsvImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Dateien (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                // Store the selected file path
                string filePath = openFileDialog.FileName;
                App.Config.Words = WordList.ImportFromCSV(filePath);
            }

            UpdateView();
        }

        /// <summary>
        /// Word verification when button pressed and show button next and hide button Check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            CheckWordMatch();
            WordInput.IsReadOnly = true;
            btnNext.Visibility = Visibility.Visible;
            btnCheck.Visibility = Visibility.Hidden;
            btnNext.Focus();
        }


        /// <summary>
        /// Word verficateion when Enter pressed and show button next and hide butotn Check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckWordMatch();
                WordInput.IsReadOnly = true;
                btnNext.Visibility = Visibility.Visible;
                btnCheck.Visibility = Visibility.Hidden;
                btnNext.Focus();
            }
        }

        /// <summary>
        /// Clear input, hide button Next and show button Check
        /// change border and background color back to standard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            WordInput.IsReadOnly = false;
            btnNext.Visibility = Visibility.Hidden;
            btnCheck.Visibility = Visibility.Visible;
            WordInput.Focus();
            WordInput.BorderBrush = Brushes.Black;
            WordInput.Background = Brushes.White;
            UpdateView();

        }

        /// <summary>
        /// Check word match from label and input and 
        /// change color to the corresponding input
        /// right = green false = red
        /// </summary>

        private void CheckWordMatch()
        {
            if (App.Config.Words.GetNextWord().CheckAnswer(WordInput.Text))
            {
                WordInput.BorderBrush = Brushes.Green;
                WordInput.Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            }
            else
            {
                WordInput.BorderBrush = Brushes.Red;
                WordInput.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            }
        }
    }
}
