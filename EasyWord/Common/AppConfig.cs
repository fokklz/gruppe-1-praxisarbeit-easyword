using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWord.Common
{
    public class AppConfig
    {
        public static string DEFAULT_LANGUAGE = "Englisch";
        public static string DEFAULT_LECTURE = "Standard";

        /// <summary>
        /// private Language storage
        /// </summary>
        private string _language = DEFAULT_LANGUAGE;
      
        /// <summary>
        /// Version of the Application
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Version date for the Application
        /// </summary>
        public DateTime VersionDate { get; set; }

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
        /// The Language the user is Currently trying to learn
        /// Resets Lectures when setted
        /// </summary>
        public string Language {
            get {
                return _language;
            }
            set {
                _language = value;
                Lectures.Clear();
            } 
        }

        /// <summary>
        /// Currently active lectures
        /// </summary>
        public HashSet<string> Lectures { get; set; } = new HashSet<string>();

        /// <summary>
        /// All stored words
        /// </summary>
        public Storage Storage { get; set; } = new Storage();

        /// Default Ctor for XML Serialization
        /// </summary>
        public AppConfig()
        {
            Version = "preview";
        }
    }
}
