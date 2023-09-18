using EasyWord.Common;
using EasyWord.Data.Models;
using Microsoft.Win32;
using System;
using System.Globalization;
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
using static System.Collections.Specialized.BitVector32;
using System.Collections.ObjectModel;
using EasyWord.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyWord.Pages
{
    /// <summary>
    /// Interaction logic for Lectures.xaml
    /// </summary>
    public partial class Lectures : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Active Lectures as Count
        /// </summary>
        public int ActiveCount
        {
            get => _activeCount;
            set
            {
                _activeCount = value;
                OnPropertyChanged();
                OnPropertyChanged("ButtonLabel");
            }
        }
        private int _activeCount = 0;

        /// <summary>
        /// Button Label to Display
        /// 
        /// Will also disable the button if no lectures are selected
        /// </summary>
        public string ButtonLabel
        {
            get
            {
                if(ActiveCount == 0)
                {
                    SelectedLecture.IsEnabled = false;
                    return "Lektionen Wählen";
                }
                SelectedLecture.IsEnabled = true;
                if (ActiveCount > 1)
                {
                    return $"{ActiveCount} Lektionen Lernen";
                }
                else
                {

                    return $"{ActiveCount} Lektion Lernen";
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Lectures()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateView()
        {
            int index = 0;
            int activeIndex = 0;
            FilledComboBox.Items.Clear();
            foreach (var item in App.Storage.GetAvailableLanguagesAsComboItem())
            {
                if (item.Content == null) continue;
                string? lang = item.Content?.ToString();
                if (lang == null) continue;
                if(lang.Equals(App.Config.Language, StringComparison.OrdinalIgnoreCase))
                {
                    activeIndex = index;
                    item.IsSelected = true;
                }
                FilledComboBox.Items.Add(item);
                index++;
            }
            FilledComboBox.SelectedIndex = activeIndex;
        }

        /// <summary>
        /// Hook will be called on Language selection Change
        /// 
        /// Will be triggered atlaest once on ViewUpdate
        /// </summary>
        private void _updateLectures()
        {
            LectureWrapPanel.Children.Clear();
            foreach (var item in App.Storage.GetAvailableLecturesByLanguageAsCard(App.Language))
            {
                item.IsChecked = App.Lectures.Contains(item.Lecture);
                item.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsChecked")
                    {
                        if (item.IsChecked)
                        {
                            App.Lectures.Add(item.Lecture);
                        }
                        else
                        {
                            App.Lectures.Remove(item.Lecture);
                        }
                        ActiveCount = App.Lectures.Count;
                    }
                };
                LectureWrapPanel.Children.Add(item);
            }
            ActiveCount = App.Lectures.Count;
        }

        /// <summary>
        /// Event handler for page-size changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DynamicResizingHelper.SetMinWidths(e.NewSize.Width, ColumnStart, ColumnEnd);
            DynamicResizingHelper.SetColumns(e.NewSize.Width, LectureWrapPanel);
        }

        /// <summary>
        /// Will update the Config and the Displayed Lectures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilledComboBox.SelectedItem == null) return;
            string? lang = (FilledComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (lang == null) return;
            if(!lang.Equals(App.Config.Language, StringComparison.OrdinalIgnoreCase))
            {
                App.Config.Language = lang;
                App.SaveSettings();
            }

            _updateLectures();
        }

        /// <summary>
        /// Will set all lectures and navigate to the learning page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllLectures_Click(object sender, RoutedEventArgs e)
        {
            HashSet<string> lectures = new HashSet<string>();
            App.Config.Lectures = lectures.Concat(App.Storage.GetAvailableLecturesByLanguage(App.Language)).ToHashSet();
            App.SaveSettingsAndCreateSession();
            ViewHandler.NavigateToPage();
        }

        /// <summary>
        /// Will set the selected lectures and navigate to the learning page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedLecture_Click(object sender, RoutedEventArgs e)
        {
            HashSet<string> lectures = new HashSet<string>();
            foreach (var item in LectureWrapPanel.Children)
            {
                if (item is LectureCard card && card.IsChecked)
                {
                    lectures.Add(card.Lecture);
                }
            }
            App.Config.Lectures = lectures;
            App.SaveSettingsAndCreateSession();
            ViewHandler.NavigateToPage();
        }
    }
}
