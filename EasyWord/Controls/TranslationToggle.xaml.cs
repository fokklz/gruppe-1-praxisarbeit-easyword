using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for TranslationToggle.xaml
    /// </summary>
    public partial class TranslationToggle : UserControl, INotifyPropertyChanged
    {
        private bool _rotated = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Rotated {  
            get
            {
                return _rotated;
            } 
            set 
            {
                _rotated = value;
                _upateRotation();
            } 
        }

        public string LearningLanguage { 
            get => _learningLanguage; 
            set {
                _learningLanguage = value;
                OnPropertyChanged();
            } 
        }
        private string _learningLanguage = App.Language;



        public TranslationToggle()
        {
            InitializeComponent();
            DataContext = this;
            App.RegisterSettingsChangedEventListener(Config_SettingsChanged, true);
        }

        /// <summary>
        /// Helper to invoke property change event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Config_SettingsChanged(object? sender, Data.Models.SettingChangedEventArgs e)
        {
            if (e.Setting == "TranslationDirection" || e.Setting == "*")
            {
                _rotated = App.Config.TranslationDirection;
                _upateRotation();
            }
        }

        private void _upateRotation()
        {
            if (_rotated)
            {
                ArrowDirection.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeftThin;
            }
            else
            {
                ArrowDirection.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRightThin;
            }
        }
    }
}
