using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls
{
    public class NamedComboBox : ComboBox
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedComboBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedComboBox), new PropertyMetadata(default(GridLength)));

        static NamedComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NamedComboBox), new FrameworkPropertyMetadata(typeof(NamedComboBox)));
            IsTabStopProperty.OverrideMetadata(typeof(NamedComboBox), new FrameworkPropertyMetadata(false));
        }

        public NamedComboBox()
        {
            IsEditable = false;
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