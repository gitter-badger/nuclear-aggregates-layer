using System;
using System.Windows;
using System.Windows.Threading;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.FocusableBinding
{
    public static class FocusController
    {
        public static readonly DependencyProperty FocusablePropertyProperty =
            DependencyProperty.RegisterAttached("FocusableProperty",
                                                typeof(string),
                                                typeof(FocusController),
                                                new UIPropertyMetadata(null, OnFocusablePropertyChanged));

        private static readonly DependencyProperty MoveFocusSinkProperty =
            DependencyProperty.RegisterAttached("MoveFocusSink",
                                                typeof(MoveFocusSink),
                                                typeof(FocusController),
                                                new UIPropertyMetadata(null));

        public static string GetFocusableProperty(DependencyObject obj)
        {
            return (string) obj.GetValue(FocusablePropertyProperty);
        }

        public static void SetFocusableProperty(DependencyObject obj, string value)
        {
            obj.SetValue(FocusablePropertyProperty, value);
        }

        private static void OnFocusablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
            {
                return;
            }

            var property = (string) e.NewValue;


            element.DataContextChanged += delegate
                {
                    CreateHandler(element, property);
                };

            if (element.DataContext != null)
            {
                CreateHandler(element, property);
            }
        }

        private static void CreateHandler(DependencyObject element, string property)
        {
            var focusMover = element.GetValue(FrameworkElement.DataContextProperty) as IFocusMover;
            if (focusMover == null)
            {
                var handler = element.GetValue(MoveFocusSinkProperty) as MoveFocusSink;
                if (handler != null)
                {
                    handler.ReleaseReferences();
                    element.ClearValue(MoveFocusSinkProperty);
                }
            }
            else
            {
                var handler = new MoveFocusSink(element as UIElement, property);
                focusMover.MoveFocus += handler.HandleMoveFocus;
                element.SetValue(MoveFocusSinkProperty, handler);
            }
        }

        private class MoveFocusSink
        {
            private UIElement _element;
            private string _property;

            public MoveFocusSink(UIElement element, string property)
            {
                _element = element;
                _property = property;
            }

            internal void HandleMoveFocus(object sender, MoveFocusEventArgs e)
            {
                if (_element == null || _property != e.FocusedProperty)
                {
                    return;
                }

                // Delay the call to allow the current batch
                // of processing to finish before we shift focus.
                _element.Dispatcher.BeginInvoke((Action)(() => _element.Focus()), DispatcherPriority.Background);

            }

            internal void ReleaseReferences()
            {
                _element = null;
                _property = null;
            }
        }
    }
}