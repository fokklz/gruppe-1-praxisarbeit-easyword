﻿using EasyWord.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWord.Common
{
    /// <summary>
    /// This class represents a session of words which are currently learned
    /// It will keep a "original" Object, and shrink a current object to the current iteration
    /// Removing words which are already learned. It will also shuffle the words
    /// When all words are learned its going to increment the iteration 
    /// Then it will reset the object with the original object.
    /// 
    /// There should always only be one instance of this class, because it will keep track of the iteration
    /// And reflects the updates to the Storage
    /// 
    /// use App.Session to get the current session
    /// or App.CreateSession() to create a new session
    /// </summary>
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
        private SessionWord[]? _currentWords;

        /// <summary>
        /// iteration counter
        /// </summary>
        private int _iteration;

        /// <summary>
        /// Constructor for Session
        /// </summary>
        /// <param name="words">All words for this session</param>
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
                    // if min and max are not the same, we have to normalize the valid value
                    // since it has to be between 0 and 1 since our iteration starts at 1
                    SessionWord sw = new SessionWord(word);
                    sw.ValidSession = word.Valid - minValid;
                    return sw;
                }
            }).ToArray();

            // initialize the session if valid
            if (_words.Length > 0)
            {
                Initialize();
            }
            else
            {
                _currentWords = new SessionWord[0];
            }
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
            if(_currentWords?.Length == 0) return false;
            return _currentWords?.Where(_filterWord).Count() > 0;
        }

        /// <summary>
        /// Check if the session is initialized
        /// </summary>
        /// <returns>True or False based on the current status</returns>
        public bool IsInitialized()
        {
            return _currentWords?.Length > 0 && _iteration > 0;
        }

        /// <summary>
        /// Check if the session is empty
        /// </summary>
        /// <returns>True when no words contained</returns>
        public bool IsEmpty()
        {
            return _words.Length == 0;
        }

        /// <summary>
        /// Initialize the session
        /// 
        /// Will do nothing if the session is already initialized or empty
        /// </summary>
        public void Initialize()
        {
            if(IsEmpty()) return;
            if(IsInitialized()) return;
            _iteration = 1;
            _currentWords = _words.Where(_filterWord).OrderBy(x => _random.Next()).ToArray();
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
        /// Bucket accesstor for the BucketDisplay
        /// </summary>
        /// <returns>Array with the number of Buckets for each Bucket</returns>
        public int[] GetBuckets()
        {
            return new int[5] {
                _words.Where(w => w.Bucket == 1).Count(),
                _words.Where(w => w.Bucket == 2).Count(),
                _words.Where(w => w.Bucket == 3).Count(),
                _words.Where(w => w.Bucket == 4).Count(),
                _words.Where(w => w.Bucket == 5).Count()
            };
        }

        /// <summary>
        /// Switch to the next word in the list and increment 
        /// the iteration stat of the word which was lately done
        /// 
        /// Will return a empty word if the session is not initialized.
        /// </summary>
        /// <returns>The current word or a empty word</returns>
        public SessionWord GetNextWord()
        {
            if (!IsInitialized()) return new SessionWord(new Word());
            if (_currentWords?.Length != 0)
            {
                return _currentWords?.First() ?? new SessionWord(new Word());
            }
            return new SessionWord(new Word());
        }

        /// <summary>
        /// Advances to the next set of words for the current iteration, reshuffling the words. 
        /// If all words have been iterated over in the current iteration, it increments the iteration counter 
        /// and reshuffles the words for the next iteration.
        /// 
        /// Will do nothing if the session is not initialized.
        /// </summary>
        public void GoNext()
        {
            if (!IsInitialized()) return;
            if (HasWordsLeft())
            {
                _currentWords = _currentWords?.Where(_filterWord).OrderBy(x => _random.Next()).ToArray();
            }
            else
            {
                _iteration++;
                _currentWords = _words.Where(_filterWord).OrderBy(x => _random.Next()).ToArray();
            }
        }

    }
}