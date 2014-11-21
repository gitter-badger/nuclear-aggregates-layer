using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Behaviors
{
    public static class SelectionBehavior
    {
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.RegisterAttached("Selection",
                                                typeof(object[]),
                                                typeof(SelectionBehavior),
                                                new FrameworkPropertyMetadata(new object[0], SelectionPropertyChanged));

        public static object[] GetSelection(DependencyObject d)
        {
            return (object[])d.GetValue(SelectionProperty);
        }

        public static void SetSelection(DependencyObject d, object[] value)
        {
            d.SetValue(SelectionProperty, value);
        }

        private static void SelectionPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var selector = target as Selector;
            if (selector == null)
            {
                return;
            }

            if ((e.NewValue != e.Property.DefaultMetadata.DefaultValue) && (e.OldValue == e.Property.DefaultMetadata.DefaultValue))
            {
                selector.SelectionChanged += OnSelectionChanged;
            }
            else if ((e.NewValue == e.Property.DefaultMetadata.DefaultValue) && (e.OldValue != e.Property.DefaultMetadata.DefaultValue))
            {
                selector.SelectionChanged -= OnSelectionChanged;
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;
            if (selector == null)
            {
                return;
            }

            var selection = GetSelectedItems(selector);
            SetSelection(selector, selection);
        }

        private static object[] GetSelectedItems(Selector selector)
        {
            var multiSelector = selector as MultiSelector;
            if (multiSelector != null)
            {
                return Convert2Array(multiSelector.SelectedItems);
            }

            var listBox = selector as ListBox;
            if (listBox != null)
            {
                return Convert2Array(listBox.SelectedItems);
            }

            var selectedItem = selector.SelectedItem;
            return selectedItem != null
                       ? new []{ selector.SelectedItem }
                       : new object[0];
        }

        private static object[] Convert2Array(IList list)
        {
            if (list == null || list.Count == 0)
            {
                return new object[0];
            }

            var array = new object[list.Count];
            list.CopyTo(array, 0);
            return array;
        }
    }
}
