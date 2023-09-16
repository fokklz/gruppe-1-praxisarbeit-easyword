using EasyWord.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWord.Common
{
    public class Session
    {
        /// <summary>
        /// Random class for shuffling the words
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// All words in the session
        /// </summary>
        private SessionWord[] _words;

        /// <summary>
        /// Current iteration of the session
        /// </summary>
        private SessionWord[] _currentWords;

        /// <summary>
        /// iteration counter
        /// </summary>
        private int _iteration;

        /// <summary>
        /// Constructor for Session
        /// </summary>
        /// <param name="words"></param>
        public Session(Word[] words)
        {
            int minValid = words.Min(w => w.Valid);
            int maxValid = words.Max(w => w.Valid);
            _words = words.Select(word =>
            {
                if (maxValid == minValid)
                {
                    return new SessionWord(word);
                }
                else
                {
                    SessionWord sw = new SessionWord(word);
                    sw.ValidSession = word.Valid - minValid;
                    return sw;
                }
            }).ToArray();

            _iteration = 1;
            _currentWords = _words.Where(_filterWord).OrderBy(x => _random.Next()).ToArray();
        }

        /// <summary>
        /// Filter the words for the current iteration
        /// </summary>
        /// <param name="word">The word to filter</param>
        /// <returns>True when keeped</returns>
        private bool _filterWord(SessionWord word)
        {
            return word.Bucket > 1 && _iteration - 1 == word.ValidSession;
        }

        /// <summary>
        /// Check if there are words left in the current iteration
        /// </summary>
        /// <returns>True when there are words left to iterate over it</returns>
        public bool HasWordsLeft()
        {
            return _currentWords.Length > 0;
        }

        /// <summary>
        /// Check if the session is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _words.Length > 0;
        }

        /// <summary>
        /// Count words based on bucket  
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public int GetBucketWords(int bucket)
        {
            return _words.Where(w => w.Bucket == bucket).Count();
        }

        /// <summary>
        /// Switch to the next word in the list and increment 
        /// the iteration stat of the word which was lately done
        /// </summary>
        /// <returns>The current word or a empty word</returns>
        public SessionWord GetNextWord()
        {
            if (_currentWords.Length != 0)
            {
                return _currentWords.First();
            }
            return new SessionWord(new Word());
        }

        /// <summary>
        /// Advances to the next set of words for the current iteration, reshuffling the words if necessary. 
        /// If the initial parameter is set to true, it initializes the current iteration with a random order of words that meet the filter criteria.
        /// If all words have been iterated over in the current iteration, it increments the iteration counter and reshuffles the words for the next iteration.
        /// </summary>
        public void GoNext()
        {
            if (_words.Length == 0) return;
            if (HasWordsLeft())
            {
                _currentWords = _currentWords.Where(_filterWord).OrderBy(x => _random.Next()).ToArray();
            }
            else
            {
                _iteration++;
                _currentWords = _words.Where(_filterWord).OrderBy(x => _random.Next()).ToArray();
            }
        }


    }
}
