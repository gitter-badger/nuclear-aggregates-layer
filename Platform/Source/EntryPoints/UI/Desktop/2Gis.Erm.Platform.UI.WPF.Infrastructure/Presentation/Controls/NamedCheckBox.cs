using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls
{
    public class NamedCheckBox : CheckBox
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedCheckBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedCheckBox), new PropertyMetadata(default(GridLength)));

        static NamedCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NamedCheckBox), new FrameworkPropertyMetadata(typeof(NamedCheckBox)));
            IsTabStopProperty.OverrideMetadata(typeof(NamedCheckBox), new FrameworkPropertyMetadata(false));
        }

        public string PropertyName
        {
            get
            {
                return (string)GetValue(PropertyNameProperty);
            }

            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        public GridLength PropertyNameRegionWidth
        {
            get
            {
                return (GridLength)GetValue(PropertyNameRegionWidthProperty);
            }

            set
            {
                SetValue(PropertyNameRegionWidthProperty, value);
            }
        }
    }
}