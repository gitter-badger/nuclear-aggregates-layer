using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Behaviors
{
    /// <summary>
    /// Используется для грида типа DataGrid
    /// </summary>
    public static partial class DataGridBehaviors
    {
        public static readonly DependencyProperty SortCommandProperty =
            DependencyProperty.RegisterAttached("SortCommand", typeof(ICommand), typeof(DataGridBehaviors), new PropertyMetadata(SortCommandChanged));

        public static void SetSortCommand(UIElement element, ICommand value)
        {
            element.SetValue(SortCommandProperty, value);
        }

        public static ICommand GetSortCommand(UIElement element)
        {
            return (ICommand)element.GetValue(SortCommandProperty);
        }

        private static void SortCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = dependencyObject as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                dataGrid.Sorting += OnSorting;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                dataGrid.Sorting -= OnSorting;
            }
        }

        private static void OnSorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            var command = GetSortCommand(dataGrid);
            if (command != null)
            {
                command.Execute(new SortingDescriptor {Column = e.Column.SortMemberPath, Direction = e.Column.SortDirection});
            }
        }
    }
}