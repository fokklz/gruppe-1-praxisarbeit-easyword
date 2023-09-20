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
    /// Interaction logic for TranslationToggle.xaml
    /// </summary>
    public partial class TranslationToggle : UserControl
    {
        private bool _rotated = false; 
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

        public string LearningLanguage { get; set; } = App.Language;



        public TranslationToggle()
        {
            InitializeComponent();
            DataContext = this;
            App.SessionUpdated += (sender, e) =>
            {
                LearningLanguage = App.Language;
            };

            App.Config.SettingsChanged += (sender, e) =>
            {
                
                if (e.Setting == "TranslationDirection")
                {
                    _rotated = App.Config.TranslationDirection;
                    _upateRotation();
                }

            };

            App.ConfigChanged += (sender, e) =>
            {
                App.Config.SettingsChanged += (sender, e) =>
                {
                    
                    if (e.Setting == "TranslationDirection")
                    {
                        _rotated = App.Config.TranslationDirection;
                        _upateRotation();
                    }

                };
            };
           _upateRotation();

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
