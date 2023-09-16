using EasyWord.Common;
using EasyWord.Data.Repository;
using System;
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
        public static AppConfig Config { get; private set; }
        public static Session Session
        {
            get => _session;
            set
            {
                _session = value;
                OnSessionChanged();
            }
        }

        private static Session _session;

        public static event EventHandler SessionChanged;

        public App()
        {
            try
            {
                Config = FileProvider.LoadConfig<AppConfig>("config.xml", true);
            } catch {
                // Use default if any errors while import
                Config = new AppConfig(); 
            }

            try
            {
                Config.Version = VersionProvider.getVersion();
            } catch
            {
                // set to unknown if any errors while reading
                Config.Version = "unknown";
            }
        }

        private static void OnSessionChanged()
        {
            SessionChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// hook to simplify settings save
        /// </summary>
        public static void SaveSettings()
        {
            FileProvider.SaveConfig(Config, "config.xml");
        }

        /// <summary>
        /// Create a new Session based on current settings
        /// </summary>
        public static void CreateSession()
        {
            Session = new Session(Config.Storage.GetWordsByLanguageAndLectures(Config.Language, Config.Lectures.ToList()));
        }

        /// <summary>
        /// Simplyfied check if the session is alive
        /// </summary>
        /// <returns></returns>
        public static bool IsAlive()
        {
            if(Session == null) return false;
            return Session.HasWordsLeft();
        }
    }
}
