using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyWord;
using EasyWord.Common;

namespace EasyWord.Data.Models
{
    /// <summary>
    /// Represents a word with its translations
    /// also tracks the stats of the word
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Foreign language translation
        /// </summary>
        private string _foreignWord;

        /// <summary>
        /// Definition of foreign language
        /// </summary>
        private string _language;

        /// <summary>
        /// Definition in which lecture it is
        /// </summary>
        private string _lecture;

        /// <summary>
        /// german translation
        /// </summary>
        private string _german;

        /// <summary>
        /// current query iteration
        /// </summary>
        protected int _iteration;

        /// <summary>
        /// amount of correct querys
        /// </summary>
        protected int _valid;

        /// <summary>
        /// current bucket position
        /// </summary>
        protected int _bucket;

        /// <summary>
        /// GUID to identify the word while running
        /// </summary>
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="german"></param>
        /// <param name="translation"></param>
        /// <param name="language"></param>
        /// <param name="lecture"></param>
        public Word(string german, string translation, string language, string lecture)
        {
            _foreignWord = translation;
            _german = german;
            if(language == string.Empty)
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
        public Word (string german, string translation, string language) : this(german , translation, language, string.Empty) { }


        /// <summary>
        /// Empty ctor for XML serialization
        /// </summary>
        public Word() : this(string.Empty,string.Empty,string.Empty,string.Empty) { }
        
        /// <summary>
        /// Allow access to private ID
        /// its a function, because the ID should not be serialized
        /// </summary>
        /// <returns>The GUID of the word</returns>
        public Guid GetID()
        {
            return _id;
        }

        /// <summary>
        /// get translation to ask for
        /// </summary>
        /// <returns></returns>
        public string Question {
            get
            {
                return App.Config.TranslationDirection ? _foreignWord : _german;
            }
        }

        /// <summary>
        /// get translation
        /// </summary>
        /// <returns></returns>
        public string Translation
        {
            get
            {
                return App.Config.TranslationDirection ? _german : _foreignWord;
            }
        }

        /// <summary>
        /// check if the translation provided is valid
        /// </summary>
        /// <param name="awnser"></param>
        /// <returns></returns>
        public bool CheckAnswer(string awnser)
        {
            Iteration++;
            if (string.Equals(awnser, Translation, 
                App.Config.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
            {
                // if answer was correct, increment the valid stat and the iteration stat
                // also decrement the bucket (bucket 1 == learned completely
                Valid++;
                if(Bucket > 1)
                {
                    Bucket--;
                }
                return true;
            }
            // if answer was wrong, increment the bucket (max. 5)
            if(Bucket < 5)
            {
                Bucket++;
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
        /// Get the changed values and then set these to the correct variable
        /// deoending on the translation direction in AppConfig.cs
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="changeWordInput"></param>
        /// <param name="changeTranslationInput"></param>
        public void EditWord(string changeWordInput, string changeTranslationInput)
        {
            // Access the TranslationDirection property from the global AppConfig instance
            bool direction = App.Config.TranslationDirection;

            if (direction)
            {
                ForeignWord = changeWordInput;
                German = changeTranslationInput;
            }
            else
            {
                ForeignWord = changeTranslationInput;
                German = changeWordInput;
            }
        }

        /// <summary>
        /// convert word to CSV
        /// </summary>
        /// <returns>CSV line</returns>
        public string ToCSV()
        {
            if (_lecture != string.Empty)
            {
                return $"{_lecture};{_german};{_foreignWord}";
            }
            else
            {
                return $"{_german};{_foreignWord}";
            }
        }

        /// <summary>
        /// Compares two strings with ignoring case sensitive
        /// </summary>
        /// <param name="val">First String</param>
        /// <param name="value">Secound String</param>
        /// <returns>True if they are equal</returns>
        private bool _compareIgnoreCase(string val, string value)
        {
            return val.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Helper to check if a word is already in the list
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool Compare(Word word)
        {
            return (
                _compareIgnoreCase(German, word.German) &&
                _compareIgnoreCase(ForeignWord, word.ForeignWord) &&
                _compareIgnoreCase(Language, word.Language) &&
                _compareIgnoreCase(Lecture, word.Lecture));
        }

        /// <summary>
        /// Get/Set foreign word translation
        /// </summary>
        public string ForeignWord { get { return _foreignWord; } set { _foreignWord = value; } }
        /// <summary>
        /// Get/Set german translation
        /// </summary>
        public string German { get { return _german; } set { _german = value; } }
        /// <summary>
        /// Get/Set iteration
        /// </summary>
        public int Iteration { get { return _iteration; } set { _iteration = value; } }
        /// <summary>
        /// Get/Set valid
        /// </summary>
        public int Valid { get { return _valid; } set { _valid = value; } }
        /// <summary>
        /// Get/Set bucket
        /// </summary>
        public int Bucket { get { return _bucket; } set { _bucket = value; } }

        /// <summary>
        /// Get/Set language
        /// </summary>
        public string Language { get { return _language; } set { _language = value; } }

        /// <summary>
        /// Get/Set Lecture
        /// </summary>
        public string Lecture { get { return _lecture; } set { _lecture = value; } }

    }
}
