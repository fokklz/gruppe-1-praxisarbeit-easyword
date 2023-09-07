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
            openFileDialog.Filter = "CSV Dateien (*.csv) |*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                // Store the selected file path
                string selectedFilePath = openFileDialog.FileName;
                App.Config.Words = WordList.ImportFromCSV(selectedFilePath);

                //Extract just the file name from the full path
                string fileName = System.IO.Path.GetFileName(selectedFilePath);

                //Update the label "title" with the selected file name
                title.Content = fileName;
            }
            else
            {
                //show message
                title.Content = "Bitte csv Datei importieren";
            }
        }
    }
}
