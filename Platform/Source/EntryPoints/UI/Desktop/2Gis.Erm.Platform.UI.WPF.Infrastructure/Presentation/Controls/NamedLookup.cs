using System.Windows;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls
{
    public class NamedLookup : LookupControl 
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedLookup), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedLookup), new PropertyMetadata(default(GridLength)));

        public static readonly DependencyProperty ReferenceProperty =
            DependencyProperty.Register("Reference", typeof(object), typeof(NamedLookup), new PropertyMetadata(default(object)));
        
        static NamedLookup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NamedLookup), new FrameworkPropertyMetadata(typeof(NamedLookup)));
            IsTabStopProperty.OverrideMetadata(typeof(NamedLookup), new FrameworkPropertyMetadata(false));
        }

        public object Reference
        {
            get { return GetValue(ReferenceProperty); }
            set { SetValue(ReferenceProperty, value); }
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

        public bool IsReadOnly { get; set; }
    }
}