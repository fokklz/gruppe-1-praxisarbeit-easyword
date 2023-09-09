﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
    }
}
