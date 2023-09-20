using EasyWord.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyWord.Common
{
    /// <summary>
    /// This is the core configuration for the application
    /// It should be used via the static instance in the App class
    /// App.Config
    /// 
    /// This class and all classes inside have to be serializable
    /// </summary>
    public class AppConfig : Settings
    {
        public static string DEFAULT_LANGUAGE = "Englisch";
        public static string DEFAULT_LECTURE = "Standard";

        /// <summary>
        /// Version of the Application
        /// </summary>
        [XmlIgnore]
        public string Version { get; set; } = "0.0.0.0";

        /// <summary>
        /// Version date for the Application
        /// </summary>
        [XmlIgnore]
        public DateTime VersionDate { get; set; }

        /// <summary>
        /// Developer information
        /// </summary>
        [XmlIgnore]
        public string[] Developer { get; } = new string[5]{
            "Arda Dursun",
            "Bobby Bilali",
            "Fokko Vos",
            "Mahir Gönen",
            "Robin Ruf"
        };

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
                Lectures = new HashSet<string>();
            }
        }
        private string _language = DEFAULT_LANGUAGE;

        /// <summary>
        /// Currently active lectures
        /// </summary>
        public HashSet<string> Lectures { get; set; } = new HashSet<string>();

        /// <summary>
        /// All stored words
        /// </summary>
        public Storage Storage { get; set; } = new Storage();

        /// </summary>
        /// Default Ctor for XML Serialization
        /// </summary>
        public AppConfig(): base() {}
    }
}
