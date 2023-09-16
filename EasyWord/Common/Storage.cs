using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using EasyWord.Data.Models;

namespace EasyWord.Common
{
    internal class Storage
    {
        private List<Word> _words = new List<Word>();

        public List<Word> Words
        {
            get { return _words; }
            set { _words = value; }
        }

        public bool HasWords
        {
            get { return _words.Count > 0; }
        }

        /// <summary>
        /// Imports data from a CSV file and creates Word objects.
        /// </summary>
        /// <param name="path">The path to the CSV file to import.</param>
        /// <returns>A Storage object containing a list of Word objects.</returns>
        /// <exception cref="Exception">Thrown if the imported file format is not as expected.</exception>
        public static void ImportFromCSV(string path)
        { // TODO: Implement storage extension

            List<Word> words = new List<Word>();
            string fileName = Path.GetFileNameWithoutExtension(path);
            string langauge = "Englisch";
            if (fileName.Contains("_"))
            {
                langauge = fileName.Split('_')[0];
            }

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            foreach (string line in lines)
            {
                string cleanLine = line.Replace("\"", "");
                if (line.Equals(string.Empty)) continue;
                if (!line.Contains(";")) continue;
                string[] parts = cleanLine.Split(';');
                if (parts.Length == 3)
                {
                    string lecture = parts[0].Trim();
                    string german = parts[1].Trim();
                    string translation = parts[2].Trim();
                    Word word = new Word(lecture,german, translation, langauge);
                    words.Add(word);
                }
                else if (parts.Length == 2)
                {
                    string german = parts[0].Trim();
                    string translation = parts[1].Trim();
                    Word word = new Word(german, translation,langauge);
                    words.Add(word);
                }
                else throw new Exception("The imported file does not contain the expected format");
            } 
            words = words
                .GroupBy(w => new { w.German, w.ForeignWord, w.Lecture, w.Language })
                .Select(group => group.First())
                .ToList();
        }
        
       public void ExportWordsWithBucketToCSV(string filePath)
       {
            foreach (string language in GetAvailableLanguages())
            {
                string fileName = $"{language}_words.csv";
                string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), fileName);
                using (StreamWriter sw = new StreamWriter(newFilePath))
                {
                    foreach (var word in GetWordsByLanguage(language))
                    {
                        
                    }
                }
            }
        }

        /// <summary>
        /// uses LINQ:
        /// With Distinct() we only get every language once - copies will be ignored
        /// Then we select every item in distinctLanguages and create a new ComboBoxItem with the language as content
        /// Then we return this Array as StringArray with every language once inside as a ComboBoxItem, so we can use it in the UI
        /// </summary>
        /// <returns></returns>
        public string[] GetAvailableLanguages()
        {
            // Check, if there are any words in the app
            if (!_words.Any())
            {
                return new string[0];
            }

            var distinctLanguages = _words.Select(w => w.Language).Distinct();
            var comboBoxItems = distinctLanguages.Select(lang => new ComboBoxItem { Content = lang }).ToArray();

            // Convert ComboBoxItem[] to string[]
            var stringArray = comboBoxItems.Select(item => item.Content.ToString()).ToArray();

            return stringArray!;
        }

        /// <summary>
        /// We get the language as parameter.
        /// Then we do a small check, if there are any words in the app already
        /// If so, we looking for every word which has the same value in its language property like the string in the parameter
        /// and add them to the filtered array.
        /// Then we will return it.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public Word[] GetWordsByLanguage(string language)
        {
            if (_words == null || string.IsNullOrEmpty(language)) return new Word[0];

            var filteredWords = _words.Where(w => w.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToArray(); // ignore case sensitive to prevent unexpected behavior
            return filteredWords;
        }

        /// <summary>
        /// Resets the bucket to the default
        /// of every word in the list
        /// </summary>
        public void ResetAllBuckets()
        {
            foreach (var word in _words)
            {
                word.ResetBucket();
            }
        }

        /// <summary>
        /// Resets the bucet to the default,
        /// but only for a specific language
        /// </summary>
        /// <param name="language"></param>
        public void ResetBuckets(string language)
        {
            foreach (var word in _words.Where(w => w.Language.Equals(language, StringComparison.OrdinalIgnoreCase)))
            {
                word.ResetBucket();
            }
        }

        /// <summary>
        /// Resets all stats of every word in the list
        /// </summary>
        public void ResetAllStatistics()
        {
            foreach (var word in _words)
            {
                word.ResetStatistic();
            }
        }

        /// <summary>
        /// Resets all stats, but only for a specific language
        /// </summary>
        /// <param name="language"></param>
        public void ResetStatistics(string language)
        {
            foreach (var word in _words.Where(w => w.Language.Equals(language, StringComparison.OrdinalIgnoreCase)))
            {
                word.ResetStatistic();
            }
        }

        /// <summary>
        /// Deletes the whole list of words, so no words are left in the app
        /// </summary>
        public void ClearAll()
        {
            _words.Clear();
        }

        /// <summary>
        /// Delete all words of a specific language
        /// </summary>
        /// <param name="language"></param>
        public void Clear(string language)
        {
            _words = _words.Where(w => !w.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
