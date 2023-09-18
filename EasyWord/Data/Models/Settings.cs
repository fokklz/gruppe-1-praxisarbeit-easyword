using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace EasyWord.Data.Models
{
    /// <summary>
    /// Arguments for the settings change event
    /// </summary>
    public class SettingChangedEventArgs: EventArgs
    {
        public string Setting {  get; set; }
        public SettingChangedEventArgs(string setting)
        {
            Setting = setting;
        }
    }

    public class Settings
    {
        /// <summary>
        /// true: EN -> DE
        /// false: DE -> EN
        /// </summary>
        public bool TranslationDirection
        {
            get
            {
                return _translationDirection;
            }
            set
            {
                _translationDirection = value;
                OnSettingsChanged();
            }
        }
        private bool _translationDirection = true;

        /// <summary>
        /// Switch between case sensitive checking
        /// </summary>
        public bool CaseSensitive
        {
            get
            {
                return _caseSensitive;
            }
            set
            {
                _caseSensitive = value;
                OnSettingsChanged();
            }
        }
        private bool _caseSensitive = false;

        /// <summary>
        /// Set current mode
        /// 0 learning
        /// 1 editing
        /// 2 creating
        /// </summary>
        [XmlIgnore]
        public int SessionMode
        {
            get
            {
                return _sessionMode;
            }
            set
            {
                _sessionMode = value;
                OnSettingsChanged();
            }
        }
        private int _sessionMode = 0;

        public event EventHandler<SettingChangedEventArgs>? SettingsChanged;

        public Settings(){}

        protected virtual void OnSettingsChanged([CallerMemberName] string propertyName = null)
        {
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs(propertyName));
        }
    }
}
