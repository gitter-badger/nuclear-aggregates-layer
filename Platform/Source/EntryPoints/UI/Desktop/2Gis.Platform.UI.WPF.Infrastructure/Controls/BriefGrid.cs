using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Controls
{
    /// <summary>
    /// The ApexGrid control is a Grid that supports easy definition of rows and columns.
    /// </summary>
    public class BriefGrid : Grid
    {
        /// <summary>
        /// Called when the rows property is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRowsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            //  Get the apex grid.
            var grid = (BriefGrid)dependencyObject;

            //  Clear any current rows definitions.
            grid.RowDefinitions.Clear();

            //  Add each row from the row lengths definition.
            foreach (var rowLength in StringLengthsToGridLengths(grid.Rows))
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = rowLength });
            }
        }

        /// <summary>
        /// Called when the columns property is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColumnsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            //  Get the apex grid.
            var grid = (BriefGrid)dependencyObject;

            //  Clear any current column definitions.
            grid.ColumnDefinitions.Clear();

            //  Add each column from the column lengths definition.
            foreach (var columnLength in StringLengthsToGridLengths(grid.Columns))
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = columnLength });
            }
        }

        /// <summary>
        /// Turns a string of lengths, such as "3*,Auto,2000" into a set of gridlength.
        /// </summary>
        /// <param name="lengths">The string of lengths, separated by commas.</param>
        /// <returns>A list of GridLengths.</returns>
        private static IEnumerable<GridLength> StringLengthsToGridLengths(string lengths)
        {
            //  Create the list of GridLengths.
            var gridLengths = new List<GridLength>();

            //  If the string is null or empty, this is all we can do.
            if (string.IsNullOrEmpty(lengths))
            {
                return gridLengths;
            }

            //  Split the string by comma.
            string[] theLengths = lengths.Split(',');

            //  Create a grid length converter.
            var gridLengthConverter = new GridLengthConverter();

            //  Use the grid length converter to set each length.
            foreach (var length in theLengths)
            {
                gridLengths.Add((GridLength)gridLengthConverter.ConvertFromString(length));
            }

            //  Return the grid lengths.
            return gridLengths;
        }

        /// <summary>
        /// The rows dependency property.
        /// </summary>
        private static readonly DependencyProperty RowsProperty = 
            DependencyProperty.Register("Rows", typeof(string), typeof(BriefGrid), new PropertyMetadata(null, OnRowsChanged));

        /// <summary>
        /// The columns dependency property.
        /// </summary>
        private static readonly DependencyProperty ColumnsProperty = 
            DependencyProperty.Register("Columns", typeof(string), typeof(BriefGrid), new PropertyMetadata(null, OnColumnsChanged));

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        [Description("The rows property.")]
        [Category("Common Properties")]
        public string Rows
        {
            get
            {
                return (string)GetValue(RowsProperty);
            }
            set
            {
                SetValue(RowsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [Description("The columns property.")]
        [Category("Common Properties")]
        public string Columns
        {
            get
            {
                return (string)GetValue(ColumnsProperty);
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }
    }
}