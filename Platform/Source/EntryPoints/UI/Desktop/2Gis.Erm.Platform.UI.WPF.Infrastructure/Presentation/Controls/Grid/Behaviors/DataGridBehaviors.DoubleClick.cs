using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Behaviors
{
    /// <summary>
    /// Используется для грида типа DataGrid
    /// </summary>
    public static partial class DataGridBehaviors
    {
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(DataGridBehaviors), new PropertyMetadata(DoubleClickCommandChanged));

        public static void SetDoubleClickCommand(UIElement element, ICommand value)
        {
            element.SetValue(DoubleClickCommandProperty, value);
        }

        public static ICommand GetDoubleClickCommand(UIElement element)
        {
            return (ICommand)element.GetValue(DoubleClickCommandProperty);
        }

        private static void DoubleClickCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = dependencyObject as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                dataGrid.MouseDoubleClick += OnMouseDoubleClick;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                dataGrid.MouseDoubleClick -= OnMouseDoubleClick;
            }
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.MouseDevice.DirectlyOver is DataGridColumnHeader)
            {
                return;
            }

            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            var selectedEntry = dataGrid.SelectedValue;
            if (selectedEntry == null)
            {
                return;
            }

            var command = GetDoubleClickCommand(dataGrid);
            if (command != null)
            {
                command.Execute(selectedEntry);
            }
        }
    }
}