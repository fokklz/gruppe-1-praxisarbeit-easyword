using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyWord.Data.Models;

namespace EasyWord.Common
{
    /// <summary>
    /// Stores a list of words
    /// </summary>
    public class WordList
    {
        /// <summary>
        /// list of Words
        /// </summary>
        private List<Word> _words;

        /// <summary>
        /// Title of the list
        /// </summary>
        private string _title;

        /// <summary>
        /// Iteration of the list
        /// </summary>
        private int _iteration = 1;

        /// <summary>
        /// Empty Ctor for Config & XML serialization
        /// </summary>
        public WordList()
        {
            _words = new List<Word>();
            _title = String.Empty;
        }

        /// <summary>
        /// Init with Predefined list
        /// </summary>
        /// <param name="words"></param>
        public WordList(List<Word> words, string title)
        {
            _words = words;
            _title = title;
        }

        /// <summary>
        /// Get all words in the current iteration
        /// </summary>
        /// <returns></returns>
        private Word[] _getIterationWords()
        {
            Word[] words = _words.Where(word => word.Bucket > 1 && _iteration > word.Valid).ToArray();
            if (words.Length == 0)
            {
                _iteration++;
                words = _words.Where(word => word.Bucket > 1 && _iteration > word.Valid).ToArray();


                _shuffleWordList();
            }

            return words;
        }

        /// <summary>
        /// Shuffle the Wordlist to randomize the order
        /// </summary>
        private void _shuffleWordList()
        {
            var random = new Random();
            _words = _words.OrderBy(x => random.Next()).ToList();
        }

        /// <summary>
        /// ImportFromCSV needs an <b>path</b>. 
        /// After you typed the path it creates an new list with validation
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static WordList ImportFromCSV(string path)
        {
            List<Word> list = new List<Word>();
            string[] lines = File.ReadAllLines(path);
            // Assume that the CSV file is valid
            bool isValidCSV = true;
            foreach (string line in lines)
            {
                string cleanLine = line;
                if (line.Equals(string.Empty)) continue;
                if (!line.Contains(";"))
                {
                    isValidCSV = false;
                    break;
                }

                if (line.Contains("\""))
                {
                    cleanLine = line.Replace("\"", "");
                }

                // Separation by semicolon
                string[] parts = cleanLine.Split(';');
                if (parts.Length != 2)
                {
                    isValidCSV = false;
                    break;
                }

                string german = parts[0].Trim();
                string english = parts[1].Trim();
                Word word = new Word(german, english);
                list.Add(word);
            }

            if (!isValidCSV)
            {
                throw new Exception("The CSV file contains invalid characters or does " +
                    "not use the expected semicolon delimiter.");
            }

            // Remove duplicates based on both German and English translations
            list = list
                .GroupBy(w => new { w.German, w.English })
                .Select(group => group.First())
                .ToList();

            return new WordList(list, Path.GetFileNameWithoutExtension(path));
        }

        /// <summary>
        /// Get/Set word list
        /// </summary>
        public List<Word> Words { get { return _words; } set { _words = value; } }

        /// <summary>
        /// Get/Set title of the list
        /// </summary>
        public string Title { get { return _title; } set { _title = value; } }

        /// <summary>
        /// Current iteration of the list
        /// </summary>
        public int Iteration { get { return _iteration; } set { _iteration = value; } } 

        /// <summary>
        /// Switch to the next word in the list and increment 
        /// the iteration stat of the word which was lately done
        /// </summary>
        /// <returns></returns>
        public (Word word, int bucket) GetNextWord()
        {
            Word[] words = _getIterationWords();
            if (words.Length != 0)
            {
                Word nextWord = words.First();
                return (nextWord, nextWord.Bucket);
            }
            return (new Word(), -1);
        }


        /// <summary>
        /// Reset the bucket value of all words in the list to 3
        /// </summary>
        public void ResetAllBuckets()
        {
            foreach (var word in _words)
            {
                word.ResetBucket();
            }
        }


        /// <summary>
        /// Resets the statistics of every word in the list and clears the whole list.
        /// </summary>
        public void ResetWordsAndStatistics()
        {
            foreach (var word in _words)
            {
                word.ResetStatistic();
            }
            _words.Clear();
            _iteration = 1;
        }

        /// <summary>
        /// Extends the imported CSV Wordlist
        /// </summary>
        /// <param name="path"></param>
        public void ExtendFromCSV(string path)
        {
            try
            {
                WordList importedWordList = WordList.ImportFromCSV(path);
                if (importedWordList != null)
                {
                    // Merge the imported word list into the existing word list
                    this.Words.AddRange(importedWordList.Words);

                    // Optionally, remove duplicates based on both German and English translations
                    this.Words = this.Words
                        .GroupBy(w => new { w.German, w.English })
                        .Select(group => group.First())
                        .ToList();
                }
                else
                {
                    Console.WriteLine("Error importing words from CSV.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extending word list from CSV: {ex.Message}");
            }
        }
    }
}
