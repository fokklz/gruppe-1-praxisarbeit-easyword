using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyWord;


namespace EasyWord.Data.Models
{
    /// <summary>
    /// Represents a Word with its translations
    /// also Tracks the stats of the word
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Foreign language translation
        /// </summary>
        private string _foreignWord = "";

        /// <summary>
        /// Definition of foreign language
        /// </summary>
        private string _language = "";

        /// <summary>
        /// Definition in which lecture it is
        /// </summary>
        private string _lecture = "";

        /// <summary>
        /// german translation
        /// </summary>
        private string _german = "";

        /// <summary>
        /// current query iteration
        /// </summary>
        private int _iteration = 0;

        /// <summary>
        /// amount of correct querys
        /// </summary>
        private int _valid = 0;

        /// <summary>
        /// current bucket position
        /// </summary>
        private int _bucket = 3;

        /// <summary>
        /// Construktor for Word
        /// </summary>
        /// <param name="german"></param>
        /// <param name="translation"></param>
        /// <param name="language"></param>
        /// <param name="lecture"></param>
        public Word(string lecture , string german, string translation, string language)
        {
            _foreignWord = translation;
            _german = german;
            _language = language;
            _lecture = lecture;
        }

        /// <summary>
        /// Empty Ctor for XML serialization
        /// </summary>
        public Word()
        {
            _german = string.Empty;
            _foreignWord = string.Empty;
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
            _iteration++;
            if (string.Equals(awnser, Translation, 
                App.Config.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
            {
                // if answer was correct, increment the valid stat and the iteration stat
                // also decrement the bucket (bucket 1 == learned completely
                _valid++;
                if(_bucket > 1)
                {
                    _bucket--;
                }
                return true;
            }
            // if answer was wrong, increment the bucket (max. 5)
            if(_bucket < 5)
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
