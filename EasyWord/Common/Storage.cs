﻿using System;
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
using EasyWord.Pages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyWord.Common
{
    /// <summary>
    /// Thrown when a word already exists in the storage
    /// </summary>
    public class DuplicateWordException : Exception
    {
        public DuplicateWordException() : base("Word already exists"){}
    }

    /// <summary>
    /// Throw when the format of the CSV file is not correct
    /// </summary>
    public class FormattingException : Exception
    {
        public FormattingException() : base("Formatting is not correct") { }
    }

    /// <summary>
    /// This is the main storage class for the words, it will keep track of all words
    /// uses Fîlters to make the words available for the UI in the packages needed
    /// 
    /// This class should only be used with one instance, because it will keep track of the words
    /// It is initialized in the App.Config and can be accessed with App.Storage
    /// </summary>
    public class Storage
    {

        /// <summary>
        /// Array of Words currently in the Storage
        /// </summary>
        public Word[] Words
        {
            get { return _words; }
            set { _words = value; }
        }
        private Word[] _words;

        /// <summary>
        /// Initialize the wordlist as empty list
        /// </summary>
        public Storage()
        {
            _words = new Word[0];
        }


        /// <summary>
        /// Checks, if the list is empty or not
        /// </summary>
        public bool HasWords
        {
            get { return _words.Length > 0; }
        }

        /// <summary>
        /// Deletes a specific word in the list
        /// </summary>
        /// <param name="id">The guid of the word to delete</param>
        public void DeleteWord(Guid id)
        {
            _words = _words.Where(w => w.GetID() != id).ToArray();
        }

        /// <summary>
        /// Let us change the Data of each word
        /// The word will be identify with the uid
        /// </summary>
        /// <param name="id">The guid of the word to change</param>
        /// <param name="german">The new german content</param>
        /// <param name="translation">The new translation content</param>
        /// <param name="lecture">The new lecture for the word</param>
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
        /// Create a new Word and add it to the array
        /// </summary>
        /// <param name="german">The german version</param>
        /// <param name="translation">The foregin word</param>
        /// <param name="language">The language</param>
        /// <param name="lecture">The lecture</param>
        /// <exception cref="DuplicateWordException">Thrown when the word already exists</exception>
        public void CreateWord(string german, string translation, string language, string lecture)
        {
            Word word = new Word(german, translation, language, lecture);
            if (!HasWord(word))
            {
                List<Word> words = _words.ToList();
                words.Add(word);
                _words = words.ToArray();
            }
            else
            {
                throw new DuplicateWordException();
            }
        }

        /// <summary>
        /// Hook function:
        /// Will execute by SessionWord.cs
        /// There the iteration is handled for the current session
        /// and then will set the latest iteration value to the 
        /// list in this storage class with this
        /// </summary>
        /// <param name="id">The guid of the word</param>
        /// <param name="iteration">The iteration count to set</param>
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
        /// <param name="id">The guid of the word</param>
        /// <param name="valid">The valid count to set</param>
        public void SetValid(Guid id, int valid)
        {
            var word = _words.FirstOrDefault(w => w.GetID() == id);
            if (word != null)
            {
                word.Valid = valid;
            }
        }

        /// <summary>
        /// Hook funtion:
        /// Will execute by SessionWord.cs
        /// There the bucket is handled for the current session
        /// and then will set the latest bucket value to the
        /// list in this storage class with this
        /// </summary>
        /// <param name="id">The guid of the word</param>
        /// <param name="bucket">The bucket to set the word to</param>
        public void SetBucket(Guid id, int bucket)
        {
            var word = _words.FirstOrDefault(w => w.GetID() == id);
            if (word != null)
            {
                word.Bucket = bucket;
            }
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
        /// <exception cref="FormattingException">Thrown if the imported file format is not as expected.</exception>
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
                // clean line and skip empty or non-csv lines
                string cleanLine = line.Trim().Replace("\"", "");
                if (cleanLine.Equals(string.Empty)) continue;
                if (!cleanLine.Contains(";")) continue;

                // split line into parts
                string[] parts = cleanLine.Split(';');
                // we only support 3 or 2 parts since we expect
                // german ; translation
                // or
                // lecture ; german ; translation
                // The words will be built using these values
                Word word;

                if (parts.Length == 3)
                {
                    string lecture = parts[0].Trim();
                    string german = parts[1].Trim();
                    string translation = parts[2].Trim();
                    word = new Word(german, translation, langauge, lecture);
                }
                else if (parts.Length == 2)
                {
                    string german = parts[0].Trim();
                    string translation = parts[1].Trim();
                    word = new Word(german, translation, langauge);
                }
                // if the length not matches the expected format, throw an FormattingException
                else throw new FormattingException();

                if (HasWord(word))
                {
                    containedWords.Add(word);
                }
                else
                {
                    words.Add(word);
                }
            }

            MergeWords(words);
            // return duplicates, will be handled by the caller
            return containedWords;
        }

        /// <summary>
        /// Exports the whole list to a CSV file
        /// </summary>
        /// <param name="filePath">The location the file should be written</param>
        public void ExportWordsToCSV(string filePath)
        {
            foreach (string language in GetAvailableLanguages())
            {
                string newFileName = Path.GetFileNameWithoutExtension(filePath);
                // Cleanup if user added a language to the filename
                if (newFileName.Contains("_"))
                {
                    newFileName = newFileName.Split('_')[1];
                }

                string fileName = $"{language}_{newFileName}.csv";
                // default to desktop if no path is given
                string newFilePath = Path.Combine(
                    path1: Path.GetDirectoryName(filePath) ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                    path2: fileName);
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

        /// <summary>
        /// Get each unique available lecture by language
        /// </summary>
        /// <param name="language">The language to get the lectures for</param>
        /// <returns>The lectures of the provided language</returns>
        public string[] GetAvailableLecturesByLanguage(string language)
        {
            return GetWordsByLanguage(language).Select(w => w.Lecture).Distinct().ToArray();
        }

        /// <summary>
        /// Implementation of LectureCard to simplify implementation
        /// Also calculates the word count for each lecture
        /// </summary>
        /// <param name="language">The language to get the lectures for</param>
        /// <returns>Array of lecture cards</returns>
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
        /// <param name="language">The Language to get the words for</param>
        /// <returns>Array of words for the provided language</returns>
        public Word[] GetWordsByLanguage(string language)
        {
            if (_words == null || string.IsNullOrEmpty(language)) return new Word[0];
            var filteredWords = _words.Where(w => w.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToArray();
            return filteredWords;
        }

        /// <summary>
        /// Will return the words based on the provided language and lectures
        /// If the Lecture Configurations are empty, it will add the default lecture
        /// or the first available lecture to the lectures list
        /// </summary>
        /// <param name="language">The langauge to get the words for</param>
        /// <param name="lectures">The lectures to get</param>
        /// <returns>The filterd words based on the provided language and lectures</returns>
        public Word[] GetWordsByLanguageAndLectures(string language, List<string> lectures)
        {
            if (_words == null || string.IsNullOrEmpty(language)) return new Word[0];

            // fix empty lecture by adding the default or first available lecture
            if (lectures.Count == 0)
            {
                string[] availableLectures = GetAvailableLecturesByLanguage(language);
                string lecture = AppConfig.DEFAULT_LECTURE;
                if (!availableLectures.Contains(AppConfig.DEFAULT_LECTURE))
                {
                    lecture = availableLectures[0];
                }

                lectures.Add(lecture);
                App.Lectures.Add(lecture);
            }

            var filteredWords = _words
                .Where(w => _compareIgnoreCase(w.Language, language) && lectures.Contains(w.Lecture))
                .ToArray();
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