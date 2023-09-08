using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                string word = App.Config.TranslationDirection == 0 ? _english : _german;
                return word;
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
            string translation = App.Config.TranslationDirection == 0 ? _german : _english;
            if (string.Equals(awnser, translation, StringComparison.OrdinalIgnoreCase))
            {
                _valid++;
                _bucket++;
                return true;
            }
            _bucket--;
            return false;
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
