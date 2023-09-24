using EasyWord.Common;
using EasyWord.Data.Repository;
using EasyWord.Windows;
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

namespace EasyWord.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {

        public Settings()
        {
            InitializeComponent();
            UpdateView();
        }

        public void UpdateView()
        {
            CaseSensitive.IsChecked = App.Config.CaseSensitive;
            TranslationToggle.LearningLanguage = App.Language;
            DeleteLanguageLabel.Text = $"{App.Language} Löschen";
        }

        /// <summary>
        /// set column min width on size changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DynamicResizingHelper.SetMinWidths(e.NewSize.Width, ColumnStart, ColumnEnd);
        }

        /// <summary>
        /// on Back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.NavigateToPage();
        }

        /// <summary>
        /// Show developer info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevInfoVersion_click(object sender, RoutedEventArgs e)
        {
            DevVersion DevInfoWindow = new DevVersion();
            DevInfoWindow.ShowDialog();
        }


        /// <summary>
        /// Implementing clear words from wordlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            App.Config = new AppConfig();
            App.SaveSettingsAndCreateSession();
            App.ShowMessage("Die Applikation wurde erfolgreich zurückgesetzt!");
            UpdateView();
        }

        /// <summary>
        /// Implementing reset all from wordlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetStats_Click(object sender, RoutedEventArgs e)
        {
            App.Storage.ResetAllStatistics();
            App.SaveSettingsAndCreateSession();
            App.ShowMessage("Alle Statistiken wurden erfolgreich zurückgesetzt!");
        }


        /// <summary>
        /// Reset all buckets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetBuckets_Click(object sender, RoutedEventArgs e)
        {
            App.Storage.ResetAllBuckets();
            App.SaveSettingsAndCreateSession();

            App.ShowMessage("Alle Buckets wurden erfolgreich zurückgesetzt!");
        }

        /// <summary>
        /// Delete the selected language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLanguage_Click(object sender, RoutedEventArgs e)
        {
            string clearing = App.Language;
            App.Storage.Clear(clearing);

            string[] languages = App.Storage.GetAvailableLanguages();
            if(languages.Length > 0)
            {
                App.Config.Language = languages[0];
            }
            else
            {
                App.Config.Language = AppConfig.DEFAULT_LANGUAGE;
            }

            App.SaveSettingsAndCreateSession();

            App.ShowMessage($"Die Sprache {clearing} wurde erfolgreich gelöscht!");
            UpdateView();
        }

        /// <summary>
        /// enable for case sensitive
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The rounted event args</param>
        private void CaseSensitive_Checked(object sender, RoutedEventArgs e)
        {
            App.Config.CaseSensitive = true;
        }

        /// <summary>
        /// disable for case sensitive
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The rounted event args</param>
        private void CaseSensitive_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Config.CaseSensitive = false;
        }

        /// <summary>
        /// Handle the change of the translation direction
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The rounted event args</param>
        private void ToggleLanguageDirection_Click(object sender, RoutedEventArgs e)
        {
            if(App.Config.TranslationDirection)
            {
                App.Config.TranslationDirection = false;
            }
            else
            {
                App.Config.TranslationDirection = true;
            }
        }

        /// <summary>
        /// Export the Words from the available Wordlist as XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ExportState_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            saveFileDialog.Title = "XML Datei Exportieren";
            saveFileDialog.DefaultExt = "xml";

            // Show the SaveFileDialog and get the selected file path
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await Task.Run(() => App.ExportState(saveFileDialog.FileName));
                    MessageBox.Show($"Der Stand wurde erfolreich Exportiert, in die Datei: \n{saveFileDialog.FileName}", "Import erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Fehler beim Speichern der Datei", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Provide feedback to the user
            }
        }

        /// <summary>
        /// Import Words with stats
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The rounted event args</param>
        private async void ImportState_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog importPath = new OpenFileDialog();
            importPath.Filter = "XML Files (*.xml)|*.xml";
            importPath.Title = "XML Datei wählen";
            importPath.DefaultExt = "xml";

            if (importPath.ShowDialog() == true)
            {
                try
                {
                    await Task.Run(() => App.LoadState(importPath.FileName));
                    MessageBox.Show("Der Stand wurde erfolreich Importiert", "Import erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Fehler beim Laden der Datei", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            UpdateView();
        }
    }
}
