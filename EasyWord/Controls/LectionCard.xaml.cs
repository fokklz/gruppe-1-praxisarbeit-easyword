using EasyWord.Common;
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
    /// Interaction logic for LectureCard.xaml
    /// </summary>
    public partial class LectureCard : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Lecture to display
        /// </summary>
        public string Lecture
        {
            get => _lecture;
            set
            {
                _lecture = value;
                OnPropertyChanged();
            }
        }
        private string _lecture = "Standard";

        /// <summary>
        /// Amount of words in the lecture
        /// </summary>
        public int WordCount
        {
            get => _wordCount;
            set
            {
                _wordCount = value;
                OnPropertyChanged();
            }
        }
        private int _wordCount = 0;

        /// <summary>
        /// Is the lecture currently active
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
        private bool _isChecked = false;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public LectureCard()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Listen for click events on the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
        }
    }
}
