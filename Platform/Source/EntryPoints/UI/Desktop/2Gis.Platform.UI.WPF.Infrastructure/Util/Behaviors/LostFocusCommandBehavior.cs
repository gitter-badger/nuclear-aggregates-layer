using System;
using System.Windows;
using System.Windows.Input;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Behaviors
{
    public static class LostFocusCommandBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(LostFocusCommandBehavior), new PropertyMetadata(OnCommandChanged));


        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(LostFocusCommandBehavior), new PropertyMetadata(default(object)));

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return element.GetValue(CommandParameterProperty);
        }

        private static void OnCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = dependencyObject as UIElement;
            if (uiElement == null)
            {
                throw new NotSupportedException("LostFocus attached command is supported only for UIElement");
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                uiElement.LostFocus += UIElementOnLostFocus;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                uiElement.LostFocus -= UIElementOnLostFocus;
            }
        }

        private static void UIElementOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var element = (UIElement)sender;
            var command = GetCommand(element);
            if (command == null)
            {
                return;
            }

            var parameter = GetCommandParameter(element);

            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}