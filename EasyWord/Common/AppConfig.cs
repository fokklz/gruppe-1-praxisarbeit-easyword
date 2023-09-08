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
        /// Version of the Application
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Developer information
        /// </summary>
        public string[] Developer { get; } = new string[5]{
            "Arda Dursun",
            "Bobby Bilali",
            "Robin Ruf",
            "Fokko Vos",
            "Mahir Gönen"
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
    }
}
