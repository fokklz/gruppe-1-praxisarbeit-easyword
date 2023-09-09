using EasyWord.Common;
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
using System.Windows.Shapes;


namespace EasyWord.Windows
{
    /// <summary>
    /// Interaction for devVersion.xaml
    /// </summary>
    public partial class DevVersion : Window
    {
        public DevVersion()
        {
            InitializeComponent();
            // Create a multiline string with each developer's name on a new line
            string developerInfo = string.Join("\n", App.Config.Developer);

            // Set the content of the label with developer information and version
            DevOutput.Text = developerInfo;
            VersionOutput.Text = App.Config.Version;
        }
    }
}
