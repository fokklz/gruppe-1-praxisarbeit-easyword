﻿using EasyWord.Common;
using EasyWord.Data.Models;
using EasyWord.Data.Repository;
using EasyWord.Windows;
using MaterialDesignThemes.Wpf;
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
        #pragma warning disable CS8618 
        public static AppConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                try
                {
                    var (version, lastModified) = VersionProvider.GetVersion();
                    _config.Version = version;
                    _config.VersionDate = lastModified;
                }
                catch
                {
                    // set to unknown if any errors while reading
                    _config.Version = "unknown";
                    _config.VersionDate = new DateTime();
                }

                // register listeners again
                foreach (var listener in settingChangedListeners)
                {
                    _config.SettingsChanged += listener;
                }
                ConfigChanged?.Invoke(null, EventArgs.Empty);
                if (_config.Storage.HasWords) CreateSession();
            }
        }
        private static AppConfig _config;
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
                OnSessionUpdated();
            }
        }

        /// <summary>
        /// Listener for the next event
        /// </summary>
        private static List<EventHandler<SessionNextEventArgs>> nextEventListeners = new List<EventHandler<SessionNextEventArgs>>();
        
        /// <summary>
        /// Listener for the settings changed event
        /// </summary>
        private static List<EventHandler<SettingChangedEventArgs>> settingChangedListeners = new List<EventHandler<SettingChangedEventArgs>>();


        private static Session? _session = null;

        public static event EventHandler? SessionUpdated;

        public static event EventHandler? ConfigChanged;

        public static SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public App() {
            try
            {
                _config = FileProvider.LoadConfig<AppConfig>("config.xml", true);
            }
            catch
            {
                // Use default if any errors while import
                _config = new AppConfig();
            }
        }

        private static void OnSessionUpdated()
        {
            SessionUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// hook to simplify settings save
        /// </summary>
        public static void SaveSettings()
        {
            FileProvider.SaveConfig(Config, "config.xml", true);
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
            CreateSession();
        }

        /// <summary>
        /// Register a listener for the next event which will presist between sessions
        /// </summary>
        /// <param name="listener">The listener to add</param>
        public static void RegisterNextEventListener(EventHandler<SessionNextEventArgs> listener, bool invoke)
        {
            if (listener != null)
            {
                nextEventListeners.Add(listener);
                if(Session != null && invoke)
                {
                    Session.Next += listener;
                    listener.Invoke(null, new SessionNextEventArgs(new Word()));
                }
            }
        }
        
        /// <summary>
        /// Register a listener for the settings changed event presist between sessions
        /// </summary>
        /// <param name="listener">The listener to add</param>
        public static void RegisterSettingsChangedEventListener(EventHandler<SettingChangedEventArgs> listener, bool invoke)
        {
            if (listener != null)
            {
                settingChangedListeners.Add(listener);
                if (invoke)
                {
                    Config.SettingsChanged += listener;
                    listener.Invoke(null, new SettingChangedEventArgs("*"));
                }
            }
        }

        /// <summary>
        /// Create a new Session based on current settings and copy the invocations
        /// </summary>
        public static void CreateSession()
        {
            Session newSession = new Session(Language, Lectures.ToList());
            Session = newSession;
            if (Session!= null)
            {
                foreach (var listener in nextEventListeners)
                {
                    Session.Next += listener;
                }
                Session.GoNext();
            }
        }

        /// <summary>
        /// Opens a dialog to show the duplicated words
        /// </summary>
        /// <param name="duplicates"></param>
        /// <returns></returns>
        public static CustomDialog OpenDuplicateDialog(List<Word> duplicates)
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Owner = Current.MainWindow,
                Data = duplicates
            };
            return customDialog;
        }

        public static void ShowMessage(string message)
        {
            MessageQueue.Enqueue(message, "Close", () => { });
        }
    }
}
