using EasyWord.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EasyWord.Common
{
    public class ViewHandler
    {
        /// <summary>
        /// The main frame of the application
        /// </summary>
        public static Frame MainFrame { get; set; }

        private static readonly Settings settingsPage = new Settings();
        private static readonly Lectures lecturesPage = new Lectures();
        private static readonly Learning learningPage = new Learning();

        /// <summary>
        /// Fallback navigation to the default page
        /// </summary>
        public static void NavigateToPage()
        {
            NavigateToPage(null);
        }

        /// <summary>
        /// Trigger the navigation to a specific page
        /// </summary>
        /// <param name="view"></param>
        public static void NavigateToPage(string? view)
        {
            switch(view)
            {
                case "Settings":
                    MainFrame.Navigate(settingsPage);
                    settingsPage.UpdateView();
                    break;
                case "Lectures":
                    MainFrame.Navigate(lecturesPage);
                    lecturesPage.UpdateView();
                    break;
                default:
                    MainFrame.Navigate(learningPage);
                    learningPage.UpdateView();
                    break;
            }
        }

    }
}
