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

        /// <summary>
        /// hook to simplify settings save
        /// </summary>
        public static void SaveSettings()
        {
            FileProvider.SaveConfig(Config, "config.xml");
        }

    }
}
