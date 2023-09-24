using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EasyWord;
using EasyWord.Common;

namespace EasyWord.Data.Models
{
    /// <summary>
    /// Represents a word with its translations
    /// also tracks the stats of the word
    /// </summary>
    public class Word : INotifyPropertyChanged
    {
        /// <summary>
        /// Foreign language translation
        /// </summary>
        public string Translation { get { return _translation; } set { _translation = value; } }
        private string _translation;

        /// <summary>
        /// Definition of foreign language
        /// </summary>
        public string Language { get { return _language; } set { _language = value; } }
        private string _language;

        /// <summary>
        /// Definition in which lecture it is
        /// </summary>
        public string Lecture { get { return _lecture; } set { _lecture = value; } }
        private string _lecture;

        /// <summary>
        /// german translation
        /// </summary>
        public string German { get { return _german; } set { _german = value; } }
        private string _german;

        /// <summary>
        /// current query iteration
        /// </summary>
        public int Iteration { get { return _iteration; } set { _iteration = value; } }
        protected int _iteration;

        /// <summary>
        /// amount of correct querys
        /// </summary>
        public int Valid { get { return _valid; } set { _valid = value; } }
        protected int _valid;

        /// <summary>
        /// current bucket position
        /// </summary>
        public int Bucket { get { return _bucket; } set { _bucket = value; } }
        protected int _bucket;

        /// <summary>
        /// GUID to identify the word while running
        /// </summary>
        [XmlIgnore]
        public int SessionValid { get { return _sessionValid; } set { _sessionValid = value; } }
        private int _sessionValid;

        [XmlIgnore]
        public Guid Guid { get; private set; } = Guid.NewGuid();

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// get translation to ask for
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public string Question
        {
            get
            {
                return App.Config.TranslationDirection ? _translation : _german;
            }
            set
            {
                if(App.Config.TranslationDirection)
                {
                    _translation = value;
                    OnPropertyChanged(nameof(Translation));
                }
                else
                {
                    _german = value;
                    OnPropertyChanged(nameof(German));
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// get translation
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public string Answer
        {
            get
            {
                return App.Config.TranslationDirection ? _german : _translation;
            }
            set
            {
                if (App.Config.TranslationDirection)
                {
                    _german = value;
                    OnPropertyChanged(nameof(German));
                }
                else
                {
                    _translation = value;
                    OnPropertyChanged(nameof(Translation));
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="german"></param>
        /// <param name="translation"></param>
        /// <param name="language"></param>
        /// <param name="lecture"></param>
        public Word(string german, string translation, string language, string lecture)
        {
            _translation = translation;
            _german = german;
            if (language == string.Empty)
            {
                language = AppConfig.DEFAULT_LANGUAGE;
            }
            _language = language;
            if (lecture == string.Empty)
            {
                lecture = AppConfig.DEFAULT_LECTURE;
            }
            _lecture = lecture;
            _iteration = 0;
            _valid = 0;
            _bucket = 3;
            _sessionValid = 0;
        }

        /// <summary>
        /// Constructor with 2 items in CSV
        /// </summary>
        /// <param name="german"></param>
        /// <param name="translation"></param>
        public Word(string german, string translation) : this(german, translation, string.Empty, string.Empty) { }

        /// <summary>
        /// Constructor with 3 items in CSV
        /// </summary>
        /// <param name="german"></param>
        /// <param name="translation"></param>
        /// <param name="language"></param>
        public Word(string german, string translation, string language) : this(german, translation, language, string.Empty) { }


        /// <summary>
        /// Empty ctor for XML serialization
        /// </summary>
        public Word() : this(string.Empty, string.Empty, string.Empty, string.Empty) { }

        /// <summary>
        /// Helper method to raise the PropertyChanged event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Equals override to use the Guid for comparison
        /// </summary>
        /// <param name="obj">The Object to match</param>
        /// <returns>The result of the comparison</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Word otherWord)
            {
                return otherWord.Guid == Guid;
            }
            return false;
        }

        /// <summary>
        /// Override GetHashCode to use the Guid
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public bool CheckAnswer(string awnser)
        {
            _iteration++;
            if (string.Equals(awnser, Answer,
                App.Config.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
            {
                // if answer was correct, increment the valid stat and the iteration stat
                // also decrement the bucket (bucket 1 == learned completely
                _valid++;
                _sessionValid++;
                if (_bucket > 1)
                {
                    _bucket--;
                }
                return true;
            }
            // if answer was wrong, increment the bucket (max. 5)
            if (_bucket < 5)
            {
                _bucket++;
            }
            return false;
        }

        /// <summary>
        /// Set the Value of Bucket to 3
        /// </summary>
        public void ResetBucket()
        {
            _bucket = 3;
        }

        /// <summary>
        /// Reset all the statistics
        /// </summary>
        public void ResetStatistic()
        {
            _iteration = 0;
            _valid = 0;
            //_bucket = 3; don't needed, because this should just reset the statistics and ResetBucket() should reset the bucket only
        }

        /// <summary>
        /// convert word to CSV
        /// </summary>
        /// <returns>CSV line</returns>
        public string ToCSV()
        {
            return $"{_lecture};{_german};{_translation}";
        }

    }
}
