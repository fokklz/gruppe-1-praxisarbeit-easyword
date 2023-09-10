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
        /// english translation
        /// </summary>
        private string _english = "";

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
        /// default Ctor
        /// </summary>
        /// <param name="german"></param>
        /// <param name="english"></param>

        public Word(string german, string english)
        {
            _english = english;
            _german = german;
        }

        /// <summary>
        /// Empty Ctor for XML serialization
        /// </summary>
        public Word()
        {
            _german = string.Empty;
            _english = string.Empty;
        }

        /// <summary>
        /// get translation to ask for
        /// </summary>
        /// <returns></returns>
        public string Question {
            get
            {
                return App.Config.TranslationDirection ? _english : _german;
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
                return App.Config.TranslationDirection ? _german : _english;
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
            // false: DE -> EN, true: EN -> DE
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
                English = changeWordInput;
                German = changeTranslationInput;
            }
            else
            {
                English = changeTranslationInput;
                German = changeWordInput;
            }
        }




        /// <summary>
        /// Get/Set english translation
        /// </summary>
        public string English { get { return _english; } set { _english = value; } }
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

    }
}
