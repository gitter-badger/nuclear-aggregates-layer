using System.Windows;
using System.Windows.Input;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Behaviors
{
    public static class ClickBehavior
    {
        #region Ordinary single click
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.RegisterAttached("ClickCommand",
                typeof(ICommand),
                typeof(ClickBehavior),
                new FrameworkPropertyMetadata(OnClickChanged));

        public static ICommand GetClickCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(ClickCommandProperty);
        }

        public static void SetClickCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(ClickCommandProperty, value);
        }

        public static readonly DependencyProperty ClickCommandParameterProperty = 
            DependencyProperty.RegisterAttached("ClickCommandParameter", typeof(object), typeof(ClickBehavior), new PropertyMetadata(default(object)));

        public static void SetClickCommandParameter(UIElement element, object value)
        {
            element.SetValue(ClickCommandParameterProperty, value);
        }

        public static object GetClickCommandParameter(UIElement element)
        {
            return element.GetValue(ClickCommandParameterProperty);
        }

        private static void OnClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as FrameworkElement;
            if (element == null)
            {
                return;
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                element.MouseDown += OnMouseClick;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                element.MouseDown -= OnMouseClick;
            }
        }
        #endregion

        #region Double click
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand",
                typeof(ICommand),
                typeof(ClickBehavior),
                new FrameworkPropertyMetadata(OnDoubleClickChanged));

        public static ICommand GetDoubleClickCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(DoubleClickCommandProperty);
        }

        public static void SetDoubleClickCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(DoubleClickCommandProperty, value);
        }

        public static readonly DependencyProperty DoubleClickCommandParameterProperty = 
            DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(ClickBehavior), new PropertyMetadata(default(object)));

        public static void SetDoubleClickCommandParameter(UIElement element, object value)
        {
            element.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static object GetDoubleClickCommandParameter(UIElement element)
        {
            return element.GetValue(DoubleClickCommandParameterProperty);
        }
        
        private static void OnDoubleClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as FrameworkElement;
            if (element == null)
            {
                return;
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                element.MouseDown += OnMouseDoubleClick;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                element.MouseDown -= OnMouseDoubleClick;
            }
        }

        private static void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            TryExecuteCommand(sender, e, 1, ClickCommandProperty, ClickCommandParameterProperty);
        }
        
        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TryExecuteCommand(sender, e, 2, DoubleClickCommandProperty, DoubleClickCommandParameterProperty);
        }

        private static void TryExecuteCommand(
            object sender, 
            MouseButtonEventArgs eventArg, 
            int targetClickCount, 
            DependencyProperty commandProperty, 
            DependencyProperty parameterProperty)
        {
            if (eventArg == null || eventArg.ChangedButton != MouseButton.Left || eventArg.ClickCount != targetClickCount)
            {
                return;
            }

            var element = sender as FrameworkElement;
            var originalSender = eventArg.OriginalSource as DependencyObject;
            if (element == null || originalSender == null)
            {
                return;
            }

            var command = (ICommand)(sender as DependencyObject).GetValue(commandProperty);
            object parameter = (sender as DependencyObject).GetValue(parameterProperty);

            if (command != null)
            {
                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }

        #endregion
    }
}