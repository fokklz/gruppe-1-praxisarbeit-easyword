using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EasyWord.Common
{
    public class DynamicResizingHelper
    {
        /// <summary>
        /// Set min widths for columns
        /// </summary>
        /// <param name="windowWidth"></param>
        /// <param name="columns"></param>
        public static void SetMinWidths(double windowWidth, params ColumnDefinition[] columns)
        {
            double minWidth;

            // Decide the MinWidth based on the window width
            if (windowWidth > 1000)
            {
                minWidth = 200;
            }
            else if (windowWidth > 800)
            {
                minWidth = 150;
            }
            else if (windowWidth > 500)
            {
                minWidth = 50;
            }
            else
            {
                minWidth = 0;
            }

            foreach (var column in columns)
            {
                column.MinWidth = minWidth;
            }
        }

        public static void SetColumns(double windowWidth, UniformGrid grid)
        {
            // Decide the Grid Columns based on the window width
            if (windowWidth > 1000)
            {
                grid.Columns = 4;
            }
            else if (windowWidth > 800)
            {
                grid.Columns = 3;
            }
            else if (windowWidth > 500)
            {
                grid.Columns = 2;
            }
        }

        public static void SetFontSize(double windowWidth, double remValue, params TextBlock[] elements) 
        {
            double baseFontSize = 16.0;
            double scaleFactor = windowWidth / 300;

            // Decide the Font Size based on the window width
            foreach (var element in elements)
            {
                element.FontSize = baseFontSize * remValue * scaleFactor;
            }
        }
    }
}
