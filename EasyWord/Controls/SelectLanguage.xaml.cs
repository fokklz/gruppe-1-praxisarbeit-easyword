using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyWord.Controls
{
    /// <summary>
    /// Event arguments for language changed event
    /// </summary>
    public class LanguageChangedEventArgs : EventArgs
    {
        public string SelectedLanguage { get; }

        public LanguageChangedEventArgs(string selectedLanguage)
        {
            SelectedLanguage = selectedLanguage;
        }
    }


    /// <summary>
    /// Interaction logic for SelectLanguage.xaml
    /// </summary>
    public partial class SelectLanguage : UserControl
    {
        /// <summary>
        /// Stores languages to reduce recalculations
        /// </summary>
        private string _languages;

        /// <summary>
        /// Event handler for language changed
        /// </summary>
        public event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        public SelectLanguage()
        {
            InitializeComponent();
            DataContext = this;
            OnSessionUpdated(null, null);
            _languages = string.Join(",", App.Storage.GetAvailableLanguages());

            // add listener for updates on session change
            App.SessionUpdated += OnSessionUpdated;
        }

        /// <summary>
        /// Event handler for session changed, will update the language select
        /// If the language count changed since initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSessionUpdated(object? sender, EventArgs? e)
        {
            int index = 0;
            int activeIndex = 0;
            LanguageSelect.Items.Clear();
            // remove listener to prevent double call
            LanguageSelect.SelectionChanged -= LanguageSelect_SelectionChanged;
            foreach (var item in App.Storage.GetAvailableLanguagesAsComboItem())
            {
                if (item.Content == null) continue;
                string? lang = item.Content?.ToString();
                if (lang == null) continue;
                if (lang.Equals(App.Config.Language, StringComparison.OrdinalIgnoreCase))
                {
                    activeIndex = index;
                    item.IsSelected = true;
                }
                LanguageSelect.Items.Add(item);
                index++;
            }
            LanguageSelect.SelectedIndex = activeIndex;
            // readd listener
            LanguageSelect.SelectionChanged += LanguageSelect_SelectionChanged;
        }

        /// <summary>
        /// Will emit the language changed event
        /// with the currently selected language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageSelect.SelectedItem == null) return;
            string? lang = (LanguageSelect.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (lang == null) return;
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(lang));
        }
    }
}
