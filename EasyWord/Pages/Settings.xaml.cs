﻿using EasyWord.Common;
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
            CaseSensitive.IsChecked = App.Config.CaseSensitive;
            LearnEnglish.IsChecked = App.Config.TranslationDirection;
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
            App.Config.Words.ClearWords();
        }

        /// <summary>
        /// Implementing reset all from wordlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetStats_Click(object sender, RoutedEventArgs e)
        {
            App.Config.Words.ResetAllStatistics();
        }

        /// <summary>
        /// Export the Words from the available Wordlist as XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a SaveFileDialog
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                saveFileDialog.Title = "Export XML File";
                saveFileDialog.DefaultExt = "xml";

                // Show the SaveFileDialog and get the selected file path
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Get the selected export file path
                    string exportFilePath = saveFileDialog.FileName;

                    // Calling SaveConfig function to save as XML
                    FileProvider.SaveConfig(App.Config.Words, exportFilePath);

                    // Provide feedback to the user
                    MessageBox.Show($"Export to XML successful. File saved at: {exportFilePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to XML: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportState_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog importPath = new OpenFileDialog();
            importPath.Filter = "XML Files (*.xml)|*.xml";
            importPath.Title = "Select XML File";
            importPath.DefaultExt = "xml";

            if (importPath.ShowDialog() == true)
            {
             WordList importedWords = FileProvider.LoadConfig<WordList>(importPath.FileName);
                App.Config.Words = importedWords;
            }
        }

        /// <summary>
        /// enable for case sensitive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaseSensitive_Checked(object sender, RoutedEventArgs e)
        {
            App.Config.CaseSensitive = true;
        }

        /// <summary>
        /// disable for case sensitive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaseSensitive_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Config.CaseSensitive = false;
        }


        /// <summary>
        /// enable for translation direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LearnEnglish_Checked(object sender, RoutedEventArgs e)
        {
            App.Config.TranslationDirection = true;
        }

        /// <summary>
        /// disable for translation direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LearnEnglish_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Config.TranslationDirection = false;
        }
    }
}
