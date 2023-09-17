using EasyWord.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EasyWord.Common
{
    /// <summary>
    /// This class handles the navigation between the different pages
    /// It has to be initialized with the MainFrame of the application
    /// All properties and methods are static
    /// 
    /// When using the class a initial navigation has to be triggered
    /// </summary>
    public class ViewHandler
    {
        /// <summary>
        /// The main frame of the application
        /// </summary>
        public static Frame? MainFrame { get; set; }

        /// <summary>
        /// Initialize the Settings Page
        /// </summary>
        private static readonly Settings settingsPage = new Settings();
        /// <summary>
        /// Initialize the Lectures Page
        /// </summary>
        private static readonly Lectures lecturesPage = new Lectures();
        /// <summary>
        /// Initialize the Learning Page
        /// </summary>
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
        /// 
        /// Available pages:
        /// - Settings
        /// - Lectures
        /// - Learning as catch all
        /// </summary>
        /// <param name="view">The View to nativate to</param>
        public static void NavigateToPage(string? view)
        {
            if(MainFrame == null)
            {
                throw new NullReferenceException("MainFrame is not set");
            }
            switch (view)
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
