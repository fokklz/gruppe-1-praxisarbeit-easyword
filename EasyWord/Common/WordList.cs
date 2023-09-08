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
        /// Empty Ctor for Config & XML serialization
        /// </summary>
        public WordList()
        {
            _words = new List<Word>();
        }

        /// <summary>
        /// Init with Predefined list
        /// </summary>
        /// <param name="words"></param>
        public WordList(List<Word> words)
        {
            _words = words;
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
            return new WordList(list);
        }

        /// <summary>
        /// Get/Set word list
        /// </summary>
        public List<Word> Words { get { return _words; } set { _words = value; } }
    }
}
