using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Controls
{
    public class GroupBox : ContentControl
    {
        public string GroupName
        {
            get
            {
                return (string)GetValue(GroupNameProperty);
            }
            set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(GroupBox), null);

        public Brush GroupNameBackground
        {
            get
            {
                return (Brush)GetValue(GroupNameBackgroundProperty);
            }
            set
            {
                SetValue(GroupNameBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty GroupNameBackgroundProperty = DependencyProperty.Register("GroupNameBackground", typeof(Brush), typeof(GroupBox), null);

        public GroupBox()
        {
            this.DefaultStyleKey = typeof(GroupBox);
        }
    }
}
