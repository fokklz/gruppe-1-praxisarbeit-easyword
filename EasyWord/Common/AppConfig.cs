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
        public string Version { get; } = "0.0.0.1";

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
        /// 0: EN -> DE
        /// 1: DE -> EN
        /// </summary>
        public int TranslationDirection { get; set; } = 0;

        public WordList Words { get; set; } = new WordList();
    }
}
