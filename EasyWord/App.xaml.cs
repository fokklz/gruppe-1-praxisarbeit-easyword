using EasyWord.Common;
using EasyWord.Data.Repository;
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

        public App()
        {
            try
            {
                Config = ConfigurationHandler.LoadConfig<AppConfig>("config.xml");
                Config.Version = VersionProvider.getVersion();
            } catch {
                // Use default if any errors while import
                Config = new AppConfig(); 
                Config.Version = "unknown";
            }
        }

        /// <summary>
        /// hook to simplify settings save
        /// </summary>
        public static void SaveSettings()
        {
            ConfigurationHandler.SaveConfig(Config, "config.xml");
        }

    }
}
