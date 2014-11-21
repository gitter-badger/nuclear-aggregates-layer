using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Controls
{
    public class NamedReadonlyTextProperty : Control
    {
        public string PropertyValue
        {
            get
            {
                return (string)GetValue(PropertyValueProperty);
            }
            set
            {
                SetValue(PropertyValueProperty, value);
            }
        }

        public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof(string), typeof(NamedReadonlyTextProperty), null);

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

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedReadonlyTextProperty), null);

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

        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedReadonlyTextProperty), null);

        public NamedReadonlyTextProperty()
        {
            this.DefaultStyleKey = typeof(NamedReadonlyTextProperty);
        }
    }
}
