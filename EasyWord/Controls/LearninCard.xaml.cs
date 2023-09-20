using EasyWord.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace EasyWord.Controls
{
    /// <summary>
    /// Interaktionslogik für LearninCard.xaml
    /// </summary>
    public partial class LearninCard : UserControl, INotifyPropertyChanged
    {
        public Word Word
        {
            get => _word;
            set
            {
                _word = value;
                OnPropertyChanged();
            }
        }
        private Word _word;

        public event PropertyChangedEventHandler? PropertyChanged;

        public LearninCard()
        {
            InitializeComponent();
            DataContext = this;
            App.SessionUpdated += App_SessionChanged;
            App.ConfigChanged += App_ConfigChanged;
        }

        /// <summary>
        /// Helper to invoke property changes
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when the Configuration changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_ConfigChanged(object? sender, EventArgs e)
        {
            App.Config.SettingsChanged += (sender, e) =>
            {
                if (e.Setting == "SessionMode" || e.Setting == "TranslationDirection")
                {
                    UpdateView();
                }
            };
            UpdateView();
        }


        /// <summary>
        /// Handles session changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_SessionChanged(object? sender, EventArgs e)
        {
            if (App.Session == null) return;
            App.Session.Next += (sender, e) =>
            {
                Word = e.CurrentWord;
            };
            Word = App.Session.GetNextWord() ?? new Word();
            UpdateView();
        }

        /// <summary>
        /// Will update the view based on the current session state
        /// </summary>
        private void UpdateView()
        {
            if (App.Session != null && App.Session.IsInitialized() && App.Storage.HasWords)
            {
                BucketDisplay.Visibility = Visibility.Visible;
                BtnEdit.Visibility = Visibility.Visible;
                WrongDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                BucketDisplay.Visibility = Visibility.Hidden;
                BtnEdit.Visibility = Visibility.Hidden;
                WrongDisplay.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Handler to set Sessionmode to editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            App.Config.SessionMode = 1;
        }
    }
}
