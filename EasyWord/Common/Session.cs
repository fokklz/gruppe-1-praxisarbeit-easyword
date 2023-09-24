using EasyWord.Controls;
using EasyWord.Data.Models;
using EasyWord.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWord.Common
{
    /// <summary>
    /// EventArgs for the Next event
    /// Will supply the current word to the event
    /// </summary>
    public class SessionNextEventArgs : EventArgs
    {
        public Word CurrentWord { get; }

        public SessionNextEventArgs(Word currentWord)
        {
            CurrentWord = currentWord;
        }
    }

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
        /// Local storage for the GUID of the last word to ensure 
        /// that the same word is not displayed twice in a row
        /// </summary>
        private Guid _lastWord = Guid.Empty;

        /// <summary>
        /// Random class for shuffling the words
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Will be set on word change
        /// Contains the number of word in each bucket for this session
        /// </summary>
        public int[] Buckets { get; set; } = new int[5] { 0,0,0,0,0 };

        /// <summary>
        /// All words in the session
        /// </summary>
        protected Word[] Words
        {
            get
            {
                return _words;
            }
            set
            {
                _words = value;
                if(value.Length > 0)
                {
                    // get the min and max valid value
                    int minValid = value.Min(w => w.Valid);
                    int maxValid = value.Max(w => w.Valid);
                    // filter the words and set the session valid
                    // to ensure that words are asked at equal rates
                    _words = value.Select(w =>
                    {
                        if (maxValid == minValid)
                        {
                            w.SessionValid = 0;
                        }
                        else
                        {
                            w.SessionValid = w.Valid - minValid;
                        }
                        return w;
                    }).ToArray();

                    // update the buckets
                    Buckets = new int[5] {
                        _words.Where(w => w.Bucket == 1).Count(),
                        _words.Where(w => w.Bucket == 2).Count(),
                        _words.Where(w => w.Bucket == 3).Count(),
                        _words.Where(w => w.Bucket == 4).Count(),
                        _words.Where(w => w.Bucket == 5).Count()
                    };

                    // initialize session if not initialized
                    if(!IsInitialized())
                    {
                        Initialize();
                    }
                }
            }
        }
        private Word[] _words = new Word[0];

        /// <summary>
        /// Current iteration of the session
        /// </summary>
        private Word[]? _currentWords;

        /// <summary>
        /// Iteration counter
        /// </summary>
        private int _iteration;
        /// <summary>
        /// Language for the session
        /// </summary>
        private string _language;
        /// <summary>
        /// Lectures for the session
        /// </summary>
        private List<string> _lecures;

        public event EventHandler<SessionNextEventArgs>? Next;

        /// <summary>
        /// Default constructor for a session
        /// </summary>
        /// <param name="language">The language of the Session</param>
        /// <param name="list">The List of the Session</param>
        public Session(string language, List<string> list)
        {
            _language = language;
            _lecures = list;
            Words = App.Storage.GetWordsByLanguageAndLectures(_language, _lecures);
        }

        /// <summary>
        /// Filter the words for the current iteration
        /// </summary>
        /// <param name="word">The word to filter</param>
        /// <returns>True when keeped</returns>
        private bool _filterWord(Word word)
        {
            return word.Bucket > 1 && _iteration - 1 == word.SessionValid;
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
        /// Switch to the next word in the list and increment 
        /// the iteration stat of the word which was lately done
        /// 
        /// Will return a empty word if the session is not initialized.
        /// </summary>
        /// <returns>The current word or a empty word</returns>
        public Word? GetNextWord()
        {
            if (!IsInitialized()) return null;
            if (_currentWords?.Length != 0)
            {
                Word? word = _currentWords?.First();
                _lastWord = word?.Guid ?? Guid.Empty;
                return word;
            }
            return null;
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
            if (IsEmpty()) return;
            if (!IsInitialized()) return;
            if (HasWordsLeft())
            {
                // move last displayed word to the end of the list
                _currentWords = _currentWords?.Where(_filterWord).OrderBy(x => x.Guid == _lastWord ? 1 : 0).ThenBy(x => _random.Next()).ToArray();
            }
            else
            {
                _iteration++;
                // move last displayed word to the end of the list
                _currentWords = _words.Where(_filterWord).OrderBy(x => x.Guid == _lastWord ? 1 : 0).ThenBy(x => _random.Next()).ToArray();
                if(_currentWords.Length == 0)
                {
                    MessageBox.Show("Geschaft!!!");
                }
            }
            App.Config.SessionMode = 0;
            
            // update the buckets
            Buckets = new int[5] {
                        _words.Where(w => w.Bucket == 1).Count(),
                        _words.Where(w => w.Bucket == 2).Count(),
                        _words.Where(w => w.Bucket == 3).Count(),
                        _words.Where(w => w.Bucket == 4).Count(),
                        _words.Where(w => w.Bucket == 5).Count()
                    };
            Next?.Invoke(this, new SessionNextEventArgs(GetNextWord() ?? new Word()));
        }

        private bool _sessionFilter(Word w)
        {
            if(w.Language == _language && _lecures.Contains(w.Lecture))
            {
                return true;
            }
            return false;
        }

        public void CleanUp()
        {
            bool hasChanges = false;
            if (IsEmpty()) return;
            if (!IsInitialized()) return;
            // remove words not same lecture or language
            _words = _words.Where(w =>
            {
                if (w.Language == _language && _lecures.Contains(w.Lecture))
                {
                    return true;
                }
                hasChanges = true;
                return false;
            }).ToArray();
            _currentWords = _words.Where(w =>
            {
                if (w.Language == _language && _lecures.Contains(w.Lecture))
                {
                    return true;
                }
                hasChanges = true;
                return false;
            }).ToArray();

            if (hasChanges)
            {
                GoNext();
            }
        }
    }
}
