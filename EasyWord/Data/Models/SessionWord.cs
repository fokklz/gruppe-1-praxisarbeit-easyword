using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWord.Data.Models
{
    public class SessionWord : Word
    {
        /// <summary>
        /// Current valid querys in this session
        /// </summary>
        private int _validSession = 0;

        /// <summary>
        /// Copy from Word
        /// </summary>
        /// <param name="word">Word to create a Session instance of</param>
        public SessionWord(Word word) : base(word.German, word.ForeignWord, word.Language, word.Lecture, word.GetID())
        {

            _iteration = word.Iteration;
            _valid = word.Valid;
            _bucket = word.Bucket;
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
                _validSession++;
                if (Bucket > 1)
                {
                    Bucket--;
                }
                return true;
            }
            // if answer was wrong, increment the bucket (max. 5)
            if (Bucket < 5)
            {
                Bucket++;
            }
            return false;
        }


        /// <summary>
        /// Current valid querys in this session
        /// </summary>
        public int ValidSession
        {
            get { return _validSession; }
            set { _validSession = value; }
        }

        /// <summary>
        /// Get/Set iteration
        /// </summary>
        public new int Iteration { 
            get { return _iteration; } 
            set { 
                _iteration = value;
                App.Config.Storage.SetIteration(GetID(), value);
            } 
        }
        /// <summary>
        /// Get/Set valid
        /// </summary>
        public new int Valid { 
            get { return _valid; } 
            set { 
                _valid = value;
                App.Config.Storage.SetValid(GetID(), value);
            } 
        }
        /// <summary>
        /// Get/Set bucket
        /// </summary>
        public new int Bucket { 
            get { return _bucket; }
            set { 
                _bucket = value;
                App.Config.Storage.SetBucket(GetID(), value);
            } 
        }
    }
}
