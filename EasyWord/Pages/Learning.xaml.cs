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
using EasyWord.Windows;

namespace EasyWord.Pages
{

    /// <summary>
    /// Validation rule for the word input
    /// </summary>
    public class WordValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Word? currentWord = App.Session?.GetNextWord();
            if (currentWord?.CheckAnswer(value?.ToString() ?? "") ?? false)
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, $"Korrekt wäre: {currentWord?.Answer}");
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

        public Word ActiveWord => App.Session?.GetNextWord() ?? new Word();

        /// <summary>
        /// Initialize the component, set the first word, set the input fields and finally update the view
        /// so the user can start learning
        /// </summary>
        public Learning()
        {
            InitializeComponent();
            DataContext = this;
            App.SessionUpdated += App_SessionChanged;
        }


        private void App_SessionChanged(object? sender, EventArgs e)
        {
            //MessageBox.Show("Session changed");
            UpdateView();
        }

        /// <summary>
        /// Updates the view to reflect the current state of the application
        /// </summary>
        public void UpdateView()
        {
            if (App.Session == null || !App.Session.IsInitialized()) App.CreateSession();
            _updateTitle();
            WordOutput.Text = ActiveWord.Question;

            if (App.Session != null)
            {
                if (App.Session.IsInitialized())
                {
                    SubmitButton.IsEnabled = true;
                    WordInput.IsEnabled = true;


                    Validation.ClearInvalid(WordInput.GetBindingExpression(TextBox.TextProperty));
                    WordInput.Focus();

                    UpdateWrongOutput();
                    return;
                }
            }

            SubmitButton.IsEnabled = false;
            WordInput.IsEnabled = false;
        }

        /// <summary>
        /// Helper function to update the title of the page
        /// if there are words in the storage, the title will be the language with the cuttrently selected lectures as subtitle
        /// ask for import if there are no words in the storage
        /// </summary>
        private void _updateTitle()
        {
            string title = "Bitte csv Datei importieren";
            if (App.Config.Storage.HasWords)
            {
                title = App.Config.Language;
                SubTitle.Text = $"{string.Join(",", App.Config.Lectures)}";
                SubTitle.Visibility = Visibility.Visible;
                SubTitle.Height = double.NaN;
            }
            else
            {
                SubTitle.Visibility = Visibility.Hidden;
                SubTitle.Height = 0;
            }
            Title.Text = title;
        }

        /// Updates WrongOutputs with the number of incorrect attempts for the
        /// current word.
        /// </summary>
        private void UpdateWrongOutput()
        {
            Word currentWord = ActiveWord;
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

                    List<Word> duplicates = App.Config.Storage.ImportFromCSV(openFileDialog.FileName);

                    if(duplicates.Count > 0)
                    {
                        CustomDialog dialog = App.OpenDuplicateDialog(duplicates);
                        if (dialog.ShowDialog() == true)
                        {
                            duplicates.ForEach(dup =>
                            {
                                App.Config.Storage.Words.Where(w => st)
                            });
                            
                        }
                    }

                    // TODO: only renew when there are new words for current active language and lectures
                    // Overwrite the current session
                    App.SaveSettingsAndCreateSession();
                    // will auto update on session refresh
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
            App.Session?.GoNext();
        }

        private void BtnLectures_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.NavigateToPage("Lectures");
        }
    }
}
