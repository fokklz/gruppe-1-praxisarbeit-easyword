using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EasyWord.Data.Models;
using EasyWord.Data.Repository;

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
        /// Represents the current set of words to be iterated over in the current session. 
        /// It is a subset of the main word list, filtered based on certain criteria such as the bucket value and the valid iteration count.
        /// </summary>
        private Word[] _currentIteration;

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
            _currentIteration = new Word[0];
        }

        /// <summary>
        /// Init with Predefined list
        /// </summary>
        /// <param name="words"></param>
        public WordList(List<Word> words, string title)
        {
            _words = words;
            _title = title;
            _currentIteration = _words.Where(_filterWord).ToArray();
        }

        private bool _filterWord(Word word)
        {
            return word.Bucket > 1 && _iteration - 1 == word.Valid;
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
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
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
        /// Utility to check for Empty word list
        /// </summary>
        public bool HasWords { get { return _words.Count > 0; } }

        /// <summary>
        /// Get/Set title of the list
        /// </summary>
        public string Title { get { return _title; } set { _title = value; } }

        /// <summary>
        /// Utility to check for Empty title
        /// </summary>
        public bool HasTitle { get { return _title != String.Empty; } }

        /// <summary>
        /// Current iteration of the list
        /// </summary>
        public int Iteration { get { return _iteration; } set { _iteration = value; } }

        /// <summary>
        /// Check if words left
        /// </summary>
        /// <returns></returns>
        public bool HasWordsLeft()
        {
            if (_words.Count == 0) return false;

            foreach (var word in _words)
            {
                if (word.Bucket > 1)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Switch to the next word in the list and increment 
        /// the iteration stat of the word which was lately done
        /// </summary>
        /// <returns></returns>
        public Word GetNextWord()
        {
            if (_currentIteration.Length != 0)
            {
                return _currentIteration.First();
            }
            return new Word();
        }

        private bool _hasWordsLeft()
        {
            return _currentIteration.Where(_filterWord).Count() > 0;
        }

        /// <summary>
        /// Advances to the next set of words for the current iteration, reshuffling the words if necessary. 
        /// If the initial parameter is set to true, it initializes the current iteration with a random order of words that meet the filter criteria.
        /// If all words have been iterated over in the current iteration, it increments the iteration counter and reshuffles the words for the next iteration.
        /// </summary>
        /// <param name="inital">Indicates whether this is the initial call to set up the first iteration.</param>
        public void GoNext(bool inital = false)
        {
            Random random = new Random();
            if (inital)
            {
                if (_currentIteration.Length == 0)
                {
                    _currentIteration = _words.Where(_filterWord).OrderBy(x => random.Next()).ToArray();
                }
                return;
            }

            if (_hasWordsLeft())
            {
                _currentIteration = _currentIteration.Where(_filterWord).OrderBy(x => random.Next()).ToArray();
            }
            else
            {
                _iteration++;
                _currentIteration = _words.Where(_filterWord).OrderBy(x => random.Next()).ToArray();
            }
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
        /// Resets the statistics of every word in the list.
        /// </summary>
        public void ResetAllStatistics()
        {
            foreach (var word in _words)
            {
                word.ResetStatistic();
            }
        }

        /// <summary>
        /// Deletes the whole WordList, so the User can start again with a clean and empty app.
        /// </summary>
        public void ClearWords()
        {
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

        /// <summary>
        /// Export back to CSV
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportWordsWithBucketToCSV(string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (Word word in Words)
                    {
                        // Format each word entry as a CSV line with semicolon as delimiter
                        string csvLine = $"{word.German};{word.English};{word.Bucket}";
                        sw.WriteLine(csvLine);
                    }
                }
                Console.WriteLine("Export to CSV with buckets successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to CSV with buckets: {ex.Message}");
            }
        }

    }
}