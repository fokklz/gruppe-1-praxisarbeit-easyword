using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyWord.Data.Models
{

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

        public event EventHandler? SettingsChanged;

        public Settings(){}

        public void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
