using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWord.Common
{
    public class AppConfig
    {
        /// <summary>
        /// private Language storage
        /// </summary>
        private string _language = "Englisch";

        /// <summary>
        /// Version of the Application
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Developer information
        /// </summary>
        public string[] Developer { get; } = new string[5]{
            "Arda Dursun",
            "Bobby Bilali",
            "Fokko Vos",
            "Mahir Gönen",
            "Robin Ruf"
        };

        /// <summary>
        /// true: EN -> DE
        /// false: DE -> EN
        /// </summary>
        public bool TranslationDirection { get; set; } = true;

        /// <summary>
        /// Switch between case sensitive checking
        /// </summary>
        public bool CaseSensitive { get; set; } = false;

        /// <summary>
        /// Imported words
        /// </summary>
        public WordList Words { get; set; } = new WordList();

        /// <summary>
        /// The Language the user is Currently trying to learn
        /// Resets Lectures when setted
        /// </summary>
        public string Language {
            get {
                return _language;
            }
            set {
                _language = value;
                Lectures = new List<string>();
            } 
        }

        /// <summary>
        /// Currently active lectures
        /// </summary>
        public List<string> Lectures { get; set; } = new List<string>();

        /// <summary>
        /// All stored words
        /// </summary>
        public Storage Storage { get; set; } = new Storage();

        /// <summary>
        /// Current learning session
        /// </summary>
        public Session? Session { get; set; } = null;

    }
}
