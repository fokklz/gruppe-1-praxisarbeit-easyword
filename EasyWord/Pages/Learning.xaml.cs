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
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    public partial class Learning : Page, INotifyPropertyChanged 
    {
        /// <summary>
        /// if true button will be next
        /// </summary>
        private bool _enableNext = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The text of the input field
        /// </summary>
        public string WordInputText { get; set; } = "";

        /// Initialize the component, set the first word, set the input fields and finally update the view
        /// so the user can start learning
        /// </summary>
        public Learning()
        {
            InitializeComponent();
            DataContext = this;
            App.ConfigChanged += App_ConfigChanged;
            App.SessionUpdated += App_SessionChanged;
            App_ConfigChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Hook to simplify the event emitting
        /// </summary>
        /// <param name="propertyName">The name of the property (should be inferred when inside a setter)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Hook called when the configuration changed
        /// Will update the listeners to update the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_ConfigChanged(object? sender, EventArgs e)
        {
            App.Config.SettingsChanged += (sender, e) =>
            {
                UpdateView();

                if (App.Config.SessionMode == 0)
                {
                    DeleteLabel.Visibility = Visibility.Visible;
                    SubmitButton.IsEnabled = true;
                    WordInput.IsEnabled = true;
                }
                else
                {
                    DeleteLabel.Visibility = Visibility.Hidden;
                    SubmitButton.IsEnabled = false;
                    WordInput.IsEnabled = false;
                }
            };
            UpdateView();
        }

        /// <summary>
        /// Update the view on session changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_SessionChanged(object? sender, EventArgs e)
        {
            UpdateView();
        }

        /// <summary>
        /// Updates the view to reflect the current state of the application
        /// </summary>
        public void UpdateView()
        {
            _updateTitle();
            _updateCard();

            if (App.Session == null
                || !App.Session.IsInitialized()
                || !App.Storage.HasWords)
            {
                SubmitButton.IsEnabled = false;
                WordInput.IsEnabled = false;
                DeleteLabel.Visibility = Visibility.Hidden;
                BtnLectures.IsEnabled = false;
                return;
            }

            SubmitButton.IsEnabled = true;
            WordInput.IsEnabled = true;
            BtnLectures.IsEnabled = true;
            DeleteLabel.Visibility = Visibility.Visible;

            // reset validation and focus input
            Validation.ClearInvalid(WordInput.GetBindingExpression(TextBox.TextProperty));
            WordInput.Focus();
        }

        /// <summary>
        /// Update the card and the inputs based on the current session mode
        /// </summary>
        private void _updateCard()
        {
            if (App.Config.SessionMode > 0)
            {
                CreateOrModifyElm.Visibility = Visibility.Visible;
                LearningCardElm.Visibility = Visibility.Hidden;
            }
            else
            {
                CreateOrModifyElm.Visibility = Visibility.Hidden;
                LearningCardElm.Visibility = Visibility.Visible;
            }
            if(App.Config.SessionMode > 0)
            {
                SubmitButton.IsEnabled = false;
                WordInput.IsEnabled = false;
                return;
            }
            SubmitButton.IsEnabled = true;
            WordInput.IsEnabled = true;
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
                LearningSubTitle.Text = $"{string.Join(",", App.Config.Lectures)}";
                LearningSubTitle.Visibility = Visibility.Visible;
                LearningSubTitle.Height = double.NaN;
            }
            else
            {
                LearningSubTitle.Visibility = Visibility.Hidden;
                LearningSubTitle.Height = 0;
            }
            LearningTitle.Text = title;
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
        /// Navigate to the lectures page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLectures_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.NavigateToPage("Lectures");
        }

        /// <summary>
        /// Event handler for the "btnCsvImport" button click
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">Additional event data</param>
        private async void BtnCsvImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV Dateien (*.csv)|*.csv";

                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        List<Word> duplicates = await Task.Run(() => App.Config.Storage.ImportFromCSV(openFileDialog.FileName));

                        if (duplicates.Count > 0)
                        {
                            CustomDialog customDialog = new CustomDialog()
                            {
                                Owner = Application.Current.MainWindow,
                                Data = duplicates
                            };
                            customDialog.ShowDialog();
                        }
                    } catch (CancelByUser)
                    {
                        return;
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
              // switch to the next word, regardsless if the word was right or not.
                App.Session?.GoNext();
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
        }

        /// <summary>
        /// Label action to delete the current word with a confirmation dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Word? word = App.Session?.GetNextWord();
            if (word == null) return;

            var confirm = MessageBox.Show($"Möchtest du das Wort {word.German}/{word.Translation} wirklich löschen?", "Wort löschen", MessageBoxButton.YesNo);
            if(confirm == MessageBoxResult.No) return;

            App.Storage.DeleteWord(word);
            App.CreateSession();
            App.ShowMessage($"Das Wort {word.German}/{word.Translation} wurde erfolgreich gelöscht");
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            App.Config.SessionMode = 2;
        }
    }
}
