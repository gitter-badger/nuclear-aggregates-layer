using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Controls
{
    public class NamedComboBox : ComboBox
    {
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

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedComboBox), null);

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

        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedComboBox), null);

        public NamedComboBox()
        {
            this.DefaultStyleKey = typeof(NamedComboBox);
        }
    }
}
