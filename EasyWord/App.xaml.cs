using EasyWord.Common;
using EasyWord.Data.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            } catch {
                // Use default if any errors while import
                Config = new AppConfig();
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
