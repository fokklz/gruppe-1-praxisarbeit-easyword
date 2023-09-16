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

        public string[] GetAvailableLanguages()
        { //TODO: implement logic

            return new string[] { "Englisch" };
        }

        public Word[] GetWordsByLanguage(string language)
        { //TODO: implement logic

            return new Word[] {new Word()};
        }
        public void ResetAllBuckets()
        {
            foreach (var word in _words)
            {
                word.ResetBucket();
            }
        }

        public void ResetAllStatistics()
        {
            foreach (var word in _words)
            {
                word.ResetStatistic();
            }
        }

        public void ClearWords()
        {
            _words.Clear();
        }
    }
}
