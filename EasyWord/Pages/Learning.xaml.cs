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
    /// Validation rule for the word input
    /// </summary>
    public class WordValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Word currentWord = App.Config.Words.GetNextWord();
            if (currentWord.CheckAnswer(value?.ToString()))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, $"Korrekt wäre: {currentWord.Translation}");
            }
        }
    }

    /// <summary>
    /// Interaction logic for Learning.xaml
    /// </summary>
    public partial class Learning : Page
    {
        /// <summary>
        /// if true button will be next
        /// </summary>
        private bool _enableNext = false;

        /// <summary>
        /// The text from the WordInput
        /// </summary>
        public string WordInputText { get; set; } 

        /// <summary>
        /// Initialize the component, set the first word, set the input fields and finally update the view
        /// so the user can start learning
        /// </summary>
        public Learning()
        {
            InitializeComponent();
            App.Config.Words.GoNext(true);
            DataContext = this;
            UpdateView();
        }

        /// <summary>
        /// Updates the view to reflect the current state of the application
        /// </summary>
        private void UpdateView()
        {
            Title.Text =
                !App.Config.Words.HasTitle ? "Bitte csv Datei importieren" : App.Config.Words.Title;
            WordOutput.Text = App.Config.Words.GetNextWord().Question;

            if (App.Config.Words.HasWordsLeft())
            {
                SubmitButton.IsEnabled = true;
                WordInput.IsEnabled = true;
            }
            else
            {
                SubmitButton.IsEnabled = false;
                WordInput.IsEnabled = false;
            }

            Validation.ClearInvalid(WordInput.GetBindingExpression(TextBox.TextProperty));
            WordInput.Focus();
 
            UpdateWrongOutput();
        }

        /// <summary>
        /// Updates WrongOutputs with the number of incorrect attempts for the
        /// current word.
        /// </summary>
        private void UpdateWrongOutput()
        {
            Word currentWord = App.Config.Words.GetNextWord();
            WrongOutput.Text = $"{currentWord.Iteration - currentWord.Valid}";
        }

        /// <summary>
        /// Event handler for page-size changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DynamicResizingHelper.SetMinWidths(e.NewSize.Width, ColumnStart, ColumnEnd);
        }

        /// <summary>
        /// Navigate to the settings page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.NavigateToPage("Settings");
        }

        /// <summary>
        /// Event handler for the "btnCsvImport" button click
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">Additional event data</param>
        private void BtnCsvImport_Click(object sender, RoutedEventArgs e)
        {
            try
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
            } catch
            {
                MessageBox.Show("Die Wörter konnten nicht Importiert werden. Überprüfe die Formatierung der CSV.");
            }
        }

        /// <summary>
        /// submit typed word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_enableNext)
            { // Go Next
                WordInput.IsReadOnly = false;
                WordInput.Background = Brushes.White;
                WordInput.Text = "";
                SubmitButton.Content = "Check";
                _enableNext = false;
                UpdateView();
            }
            else
            { // Check Input
                CheckWordMatch();
            }
        }

        /// <summary>
        /// Word verification on enter key press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckWordMatch();
            }
        }

        /// <summary>
        /// Check word match from label and input and 
        /// change color to the corresponding input
        /// </summary>

        private void CheckWordMatch()
        {
            WordInput.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            // Check if there are any validation errors
            if (Validation.GetHasError(WordInput))
            {
                WordInput.BorderBrush = Brushes.Red;
                WordInput.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            }
            else
            {
                WordInput.BorderBrush = Brushes.Green;
                WordInput.Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            }
            _enableNext = true;
            SubmitButton.Content = "Next";
            WordInput.IsReadOnly = true;
            SubmitButton.Focus();
            // switch to the next word, regardsless if the word was right or not.
            App.Config.Words.GoNext(); 
        }

        private void BtnLectures_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.NavigateToPage("Lectures");
        }
    }
}
