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
                if(_originalWord != null && !_createMode)
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
            App.RegisterNextEventListener(Session_Next, true);
            App.RegisterSettingsChangedEventListener(Config_SettingsChanged, true);
        }

        private void Config_SettingsChanged(object? sender, SettingChangedEventArgs e)
        {
            if(e.Setting == "SessionMode")
            {
                if(App.Config.SessionMode == 2)
                {
                    CreateMode = true;
                    Word = new Word();
                }
                else
                {
                    CreateMode = false;
                }
            }
        }

        /// <summary>
        /// Helper to invoke property change event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Session update handler to update the current word of this control
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The params of the event</param>
        private void Session_Next(object? sender, Common.SessionNextEventArgs e)
        {
            Word = e.CurrentWord;
        }

        /// <summary>
        /// Handler for language selection change
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The params of the event</param>
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
            if(_originalWord != null)
            {
                Word.Question = _originalWord.Question;
                Word.Answer = _originalWord.Answer;
                Word.Lecture = _originalWord.Lecture;
                Word.Language = _originalWord.Language;
            }
            OnPropertyChanged("QuestionLabel");
            OnPropertyChanged("AnswerLabel");
            OnPropertyChanged("Word");
            App.Config.SessionMode = 0;
            _originalWord = null;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(!_createMode)
            {
                if (_originalWord != null)
                {
                    if (_originalWord.Lecture != Word.Lecture || _originalWord.Language != Word.Language)
                    {
                        App.Session?.CleanUp();
                    }
                }
                App.ShowMessage($"Das Wort {Word.German}/{Word.Translation} wurde erfolgreich angepasst!");
            }
            else
            {
                try
                {
                    App.Storage.CreateWord(Word);

                    if (App.Language == Word.Language && App.Config.Lectures.Contains(Word.Lecture))
                    {
                        App.SaveSettingsAndCreateSession();
                    }
                    else
                    {
                        App.SaveSettings();
                    }

                    App.ShowMessage($"Das Wort {Word.German}/{Word.Translation} wurde erfolgreich erstellt!");
                }
                catch (DuplicateWordException)
                {
                    App.ShowMessage($"Das Wort {Word.German}/{Word.Translation} existiert bereits!");
                }
            }

            App.Config.SessionMode = 0;
            _originalWord = null;
        }
    }
}