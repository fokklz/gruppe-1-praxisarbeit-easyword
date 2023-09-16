using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using EasyWord.Controls;
using EasyWord.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyWord.Common
{
    public class Storage
    {
        private Word[] _words;

        /// <summary>
        /// Initialize the wordlist as empty list
        /// </summary>
        public Storage()
        {
            _words = new Word[0];
        }

        /// <summary>
        /// Deletes a specific word in the list
        /// </summary>
        /// <param name="id"></param>
        public void DeleteWord(Guid id)
        {
            _words = _words.Where(w => w.GetID() != id).ToArray();
        }

        /// <summary>
        /// Let us change the Data of each word
        /// The word will be identify with the uid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="german"></param>
        /// <param name="translation"></param>
        /// <param name="lecture"></param>
        public void ChangeWordData(Guid id, string german, string translation, string lecture)
        {
            var word = _words.FirstOrDefault(w => w.GetID() == id);
            if (word != null)
            {
                word.German = german;
                word.ForeignWord = translation;
                word.Lecture = lecture;
            }
        }

        /// <summary>
        /// Hook function:
        /// Will execute by SessionWord.cs
        /// There the iteration is handled for the current session
        /// and then will set the latest iteration value to the 
        /// list in this storage class with this
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iteration"></param>
        public void SetIteration(Guid id, int iteration)
        {
            var word = _words.FirstOrDefault(w => w.GetID() == id);
            if (word != null)
            {
                word.Iteration = iteration;
            }
        }

        /// <summary>
        /// Hook function:
        /// Will execute by SessionWord.cs
        /// There the valid value is handled for the current session
        /// and then will set the latest valid value to the list
        /// in this storage class with this
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valid"></param>
        public void SetValid(Guid id, int valid)
        {
            var word = _words.FirstOrDefault(w => w.GetID() == id);
            if (word != null)
            {
                word.Valid = valid;
                MessageBox.Show("SetValid used"); // for testing
            }
        }

        /// <summary>
        /// Hook funtion:
        /// Will execute by SessionWord.cs
        /// There the bucket is handled for the current session
        /// and then will set the latest bucket value to the
        /// list in this storage class with this
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bucket"></param>
        public void SetBucket(Guid id, int bucket)
        {
            var word = _words.FirstOrDefault(w => w.GetID() == id);
            if (word != null)
            {
                word.Bucket = bucket;
            }
        }

        /// <summary>
        /// Getter and Setter for the wordlist
        /// </summary>
        public Word[] Words
        {
            get { return _words; }
            set { _words = value; }
        }

        /// <summary>
        /// Checks, if the list is empty or not
        /// </summary>
        public bool HasWords
        {
            get { return _words.Length > 0; }
        }

        /// <summary>
        /// Compares two strings with ignoring case sensitive
        /// </summary>
        /// <param name="val">First String</param>
        /// <param name="value">Secound String</param>
        /// <returns>True if they are equal</returns>
        private bool _compareIgnoreCase(string val, string value)
        {
            return val.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Helper to check if a word is already in the list
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool HasWord(Word word)
        {
            return _words.Any(w =>
                _compareIgnoreCase(w.German, word.German) &&
                _compareIgnoreCase(w.ForeignWord, word.ForeignWord) &&
                _compareIgnoreCase(w.Language, word.Language) &&
                _compareIgnoreCase(w.Lecture, word.Lecture));
        }


        /// <summary>
        /// Helper to merge duplicates if desired
        /// </summary>
        /// <param name="words">The words to add</param>
        public void MergeWords(List<Word> words)
        {
            List<Word> listWords = _words.ToList();
            listWords.AddRange(words);
            _words = listWords.ToArray();
        }

        /// <summary>
        /// Imports data from a CSV file and creates Word objects.
        /// </summary>
        /// <param name="path">The path to the CSV file to import.</param>
        /// <returns>A Storage object containing a list of Word objects.</returns>
        /// <exception cref="Exception">Thrown if the imported file format is not as expected.</exception>
        public List<Word> ImportFromCSV(string path)
        { // TODO: Implement storage extension

            List<Word> words = new List<Word>();
            List<Word> containedWords = new List<Word>();

            string fileName = Path.GetFileNameWithoutExtension(path);
            // initial default language
            string langauge = AppConfig.DEFAULT_LANGUAGE;
            if (fileName.Contains("_"))
            {
                // overwrite when language is set
                langauge = fileName.Split('_')[0];
            }

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            foreach (string line in lines)
            {
                string cleanLine = line.Trim().Replace("\"", "");
                if (cleanLine.Equals(string.Empty)) continue;
                if (!cleanLine.Contains(";")) continue;

                string[] parts = cleanLine.Split(';');
                if (parts.Length == 3)
                {
                    string lecture = parts[0].Trim();
                    string german = parts[1].Trim();
                    string translation = parts[2].Trim();
                    Word word = new Word(german, translation, langauge, lecture);
                    if (HasWord(word))
                    {
                        containedWords.Add(word);
                    }
                    else
                    {
                        words.Add(word);
                    }
                }
                else if (parts.Length == 2)
                {
                    string german = parts[0].Trim();
                    string translation = parts[1].Trim();
                    Word word = new Word(german, translation, langauge);
                    if (HasWord(word))
                    {
                        containedWords.Add(word);
                    }
                    else
                    {
                        words.Add(word);
                    }
                }
                else throw new Exception("The imported file does not contain the expected format");
            }

            MergeWords(words);
            return containedWords;
        }

        /// <summary>
        /// Exports the whole list to a CSV file
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportWordsToCSV(string filePath)
        {
            foreach (string language in GetAvailableLanguages())
            {
                string newFileName = Path.GetFileNameWithoutExtension(filePath);
                if (newFileName.Contains("_"))
                {
                    newFileName = newFileName.Split('_')[1];
                }
                string fileName = $"{language}_{newFileName}.csv";
                string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), fileName);
                using (StreamWriter sw = new StreamWriter(newFilePath))
                {
                    foreach (var word in GetWordsByLanguage(language))
                    {
                        sw.WriteLine(word.ToCSV());
                    }
                }
            }
        }

        /// <summary>
        /// Get each available language
        /// </summary>
        /// <returns>Returns each unique Language contained</returns>
        public string[] GetAvailableLanguages()
        {
            return _words.Select(w => w.Language).Distinct().ToArray();
        }

        /// <summary>
        /// uses GetAvailableLanguages() to get the languages and then creates a ComboBoxItem for each language
        /// </summary>
        /// <returns>ArrayOfComboBoxItems</returns>
        public ComboBoxItem[] GetAvailableLanguagesAsComboItem()
        {
            return GetAvailableLanguages().Select(lang => new ComboBoxItem { Content = lang }).ToArray();
        }

        public string[] GetAvailableLecturesByLanguage(string language)
        {
            return GetWordsByLanguage(language).Select(w => w.Lecture).Distinct().ToArray();
        }

        public LectureCard[] GetAvailableLecturesByLanguageAsCard(string language)
        {
            Word[] languageWords = GetWordsByLanguage(language);
            return GetAvailableLecturesByLanguage(language).Select(lecture =>
            {
                int wordCount = languageWords.Where(w => _compareIgnoreCase(lecture, w.Lecture)).Count();
                return new LectureCard { Lecture = lecture, WordCount = wordCount };
            }).ToArray();
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
            var filteredWords = _words.Where(w => w.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToArray();
            return filteredWords;
        }

        /// <summary>
        /// We will provide language and lecture as parameter
        /// then we can get the words filtered by the chosen
        /// language and the chosen lecture(s)
        /// </summary>
        /// <param name="language"></param>
        /// <param name="lectures"></param>
        /// <returns></returns>
        public Word[] GetWordsByLanguageAndLectures(string language, List<string> lectures)
        {
            if (_words == null || string.IsNullOrEmpty(language)) return new Word[0];


            if (lectures.Count == 0)
            {
                lectures = GetAvailableLecturesByLanguage(language).ToList();
            }

            var filteredWords = _words
                .Where(w => _compareIgnoreCase(w.Language, language) && lectures.Contains(w.Lecture))
                .ToArray();

             // for testing
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
            _words = new Word[0];
        }

        /// <summary>
        /// Delete all words of a specific language
        /// </summary>
        /// <param name="language">The language to Clear</param>
        public void Clear(string language)
        {
            _words = _words.Where(w => !_compareIgnoreCase(w.Language, language)).ToArray();
        }
    }
}