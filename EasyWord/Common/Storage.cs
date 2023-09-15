using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static Storage ImportFromCSV(string path)
        {
            List<Word> list = new List<Word>();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
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
                throw new Exception("The CSV file contains invalid characters or does not use the expected semicolon delimiter.");
            }

            list = list
                .GroupBy(w => new { w.German, w.English })
                .Select(group => group.First())
                .ToList();

            Storage storage = new Storage();
            storage.Words = list;
            return storage;
        }

        public void ExtendFromCSV(string path)
        {
            try
            {
                Storage importedStorage = Storage.ImportFromCSV(path);
                if (importedStorage != null)
                {
                    this.Words.AddRange(importedStorage.Words);
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

        public void ExportWordsWithBucketToCSV(string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (Word word in Words)
                    {
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
