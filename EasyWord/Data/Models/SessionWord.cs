using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public SessionWord(Word word) : base(word.Lecture, word.German, word.ForeignWord, word.Language)
        {

            Iteration = word.Iteration;
            Valid = word.Valid;
            Bucket = word.Bucket;
        }
        
        /// <summary>
        /// Wrapper for CheckAnswer to reflect to the session
        /// </summary>
        /// <param name="awnser">The Awnser the user Provided</param>
        /// <returns>True when Correct</returns>
        public new bool CheckAnswer(string awnser)
        {
            if (base.CheckAnswer(awnser))
            {
                _validSession++;
                return true;
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
    }
}
