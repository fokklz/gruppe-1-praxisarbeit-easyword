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
            string titleContent = "Bitte csv Datei importieren";
            if (App.Config.Words.Title.Length > 0)
            {
                titleContent = App.Config.Words.Title;
            }
            title.Content = titleContent;
            wordOutput.Content = App.Config.Words.GetNextWord().Question;
            wordInput.Text = "";
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
                // User confirmed, proceed with import
                if (App.Config.Words.Words.Count == 0)
                {
                    // If the word list is null, simply import the CSV
                    App.Config.Words = WordList.ImportFromCSV(filePath);
                }
                else
                {
                    var confirmResult = MessageBox.Show("Willst du die aktuelle Liste überschreiben?", "Confirmation", MessageBoxButton.YesNo);
                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        // If the word list is null, simply import the CSV
                        App.Config.Words = WordList.ImportFromCSV(filePath);
                    }
                    else
                    {
                        // If the word list already exists, extend it
                        WordList importedWords = WordList.ImportFromCSV(filePath);
                        App.Config.Words.ExtendFromCSV(filePath);
                    }
                }                
                    // Update the view to reflect the changes
                    UpdateView();
            }
        }

        /// <summary>
        /// Word verification when button pressed and show button next and hide button Check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            CheckWordMatch();
            wordInput.IsReadOnly = true;
            btnNext.Visibility = Visibility.Visible;
            btnCheck.Visibility = Visibility.Hidden;
            btnNext.Focus();
        }


        /// <summary>
        /// Word verficateion when Enter pressed and show button next and hide butotn Check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckWordMatch();
                wordInput.IsReadOnly = true;
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
            wordInput.IsReadOnly = false;
            btnNext.Visibility = Visibility.Hidden;
            btnCheck.Visibility = Visibility.Visible;
            wordInput.Focus();
            wordInput.BorderBrush = Brushes.Black;
            wordInput.Background = Brushes.White;
            UpdateView();

        }

        /// <summary>
        /// Check word match from label and input and 
        /// change color to the corresponding input
        /// right = green false = red
        /// </summary>

        private void CheckWordMatch()
        {
            if (App.Config.Words.GetNextWord().CheckAnswer(wordInput.Text))
            {
                wordInput.BorderBrush = Brushes.Green;
                wordInput.Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            }
            else
            {
                wordInput.BorderBrush = Brushes.Red;
                wordInput.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            }
        }
    }
}
