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
