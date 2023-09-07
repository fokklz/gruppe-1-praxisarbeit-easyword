using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWord.Common
{
    /// <summary>
    /// Wordlists with parameters
    /// </summary>
    public class WordList
    {
        private List<Word> _words;

        /// <summary>
        /// Fallback for config
        /// </summary>
        public WordList()
        {
            _words = new List<Word>();
        }

        public WordList(List<Word> words)
        {
            _words = words;
        }
        /// <summary>
        /// returns the words that are inside the called list
        /// </summary>
        /// <returns></returns>
        public List<Word> GetWords()
        {
            return _words;
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
            try
            {
                string[] lines = File.ReadAllLines(path);
                // Assume that the CSV file is valid
                bool isValidCSV = true;
                foreach (string line in lines)
                {
                    if (!line.Contains(";") || line.Contains("\""))
                    {
                        isValidCSV = false;
                        break;
                    }

                    // Separation by semicolon
                    string[] parts = line.Split(';');
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
                    Console.WriteLine("The CSV file contains invalid characters or does " +
                        "not use the expected semicolon delimiter.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the CSV file: {ex.Message}");
            }
            return new WordList(list);
        }
    }
}
