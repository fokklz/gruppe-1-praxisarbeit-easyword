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
    /// Interaktionslogik für CreateOrModify.xaml
    /// </summary>
    public partial class CreateOrModify : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Switch to creation
        /// </summary>
        public bool CreateMode
        {
            get => _createMode;
            set
            {
                _createMode = value;
                Word = new Word();
                OnPropertyChanged();
            }
        }
        private bool _createMode = false;

        /// <summary>
        /// Current word
        /// </summary>
        public Word Word
        {
            get => _word;
            set
            {
                if(_originalWord != null)
                {
                    // cancle changes on new word without saving
                    Cancel_Click(null, null);
                }
                _originalWord = new Word(value.German, value.Translation, value.Language, value.Lecture);
                _word = value;
                OnPropertyChanged();
                OnPropertyChanged("QuestionLabel");
                OnPropertyChanged("AnswerLabel");
            }
        }
        private Word _word = new Word();
        private Word? _originalWord = null;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Get label for the question
        /// based on current translation direction
        /// </summary>
        public string QuestionLabel
        {
            get
            {
                string translationLabel = CreateMode ? App.Language : Word.Language;
                return App.Config.TranslationDirection ? translationLabel : "Deutsch";
            }
        }

        /// <summary>
        /// Get label for the answer
        /// based on current translation direction
        /// </summary>
        public string AnswerLabel
        {
            get
            {
                string translationLabel = CreateMode ? App.Language : Word.Language;
                return App.Config.TranslationDirection ? "Deutsch" : translationLabel;
            }
        }

        public CreateOrModify()
        {
            DataContext = this;
            InitializeComponent();
            App.SessionUpdated += App_SessionUpdated;
        }

        /// <summary>
        /// Helper to invoke property change event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handler for app session changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_SessionUpdated(object? sender, EventArgs e)
        {
            if (App.Session == null) return;
            App.Session.Next += (sender, e) =>
            {
                Word = e.CurrentWord;
            };
            Word = App.Session.GetNextWord() ?? new Word();
        }

        /// <summary>
        /// Handler for language selection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectLanguage_LanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            Word.Language = e.SelectedLanguage;
            OnPropertyChanged("QuestionLabel");
            OnPropertyChanged("AnswerLabel");
            OnPropertyChanged("Word");
        }

        /// <summary>
        /// Handler to reset the changes and switch session mode on cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object? sender, RoutedEventArgs? e)
        {
            if(_originalWord == null) return;
            Word.Question = _originalWord.Question;
            Word.Answer = _originalWord.Answer;
            Word.Lecture = _originalWord.Lecture;
            Word.Language = _originalWord.Language;
            OnPropertyChanged("QuestionLabel");
            OnPropertyChanged("AnswerLabel");
            OnPropertyChanged("Word");
            App.Config.SessionMode = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_originalWord == null) return;
            if(_originalWord.Lecture != Word.Lecture || _originalWord.Language != Word.Language)
            {
                App.Session?.CleanUp();
            }
            App.Config.SessionMode = 0;
            _originalWord = null;
        }
    }
}