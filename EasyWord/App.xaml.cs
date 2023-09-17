using EasyWord.Common;
using EasyWord.Data.Models;
using EasyWord.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EasyWord
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// global application configuration
        /// </summary>
        #pragma warning disable CS8618 
        public static AppConfig Config { get; private set; }
        #pragma warning restore CS8618

        /// <summary>
        /// Global Storage of words
        /// Shortcut to Config.Storage
        /// </summary>
        public static Storage Storage => Config.Storage;

        /// <summary>
        /// Currently active language
        /// Shortcut to Config.Language
        /// </summary>
        public static string Language => Config.Language;

        /// <summary>
        /// Currently active lectures
        /// Shortcut to Config.Lectures
        /// </summary>
        public static HashSet<string> Lectures => Config.Lectures;

        /// <summary>
        /// Public Session with Event to notify about changes
        /// Allows for automatic view updates
        /// </summary>
        public static Session? Session
        {
            get => _session;
            set
            {
                _session = value;
                OnSessionChanged();
            }
        }

        private static Session? _session = null;

        public static event EventHandler? SessionChanged;

        public App()
        { 
            _load();
        }

        private static void OnSessionChanged()
        {
            SessionChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void _load()
        {
            try
            {
                Config = FileProvider.LoadConfig<AppConfig>("config.xml", true);
            }
            catch
            {
                // Use default if any errors while import
                Config = new AppConfig();
            }
            try
            {
                var (version, lastModified) = VersionProvider.GetVersion();
                Config.Version = version;
                Config.VersionDate = lastModified;
            }
            catch
            {
                // set to unknown if any errors while reading
                Config.Version = "unknown";
                Config.VersionDate = new DateTime();
            }

            // Initialize a Session if words are available
            if (Storage.HasWords) CreateSession();
        }

        /// <summary>
        /// hook to simplify settings save
        /// </summary>
        public static void SaveSettings()
        {
            FileProvider.SaveConfig(Config, "config.xml", true);
        }


        public static void ReloadSettings()
        {
            SaveSettings();
            _load();

        }

        public static void ExportState(string filePath)
        {
            FileProvider.SaveConfig(Config, filePath);
        }

        public static void LoadState(string filePath)
        {
            Config = FileProvider.LoadConfig<AppConfig>(filePath);
            SaveSettingsAndCreateSession();
        }

        public static void SaveSettingsAndCreateSession()
        {
            SaveSettings();
            if (Storage.HasWords) CreateSession();
        }

        /// <summary>
        /// Create a new Session based on current settings
        /// </summary>
        public static void CreateSession()
        {
            Word[] words = Storage.GetWordsByLanguageAndLectures(Language, Lectures.ToList());
            if (words.Length > 0)
            {
                Session = new Session(words);
            }
        }
    }
}
