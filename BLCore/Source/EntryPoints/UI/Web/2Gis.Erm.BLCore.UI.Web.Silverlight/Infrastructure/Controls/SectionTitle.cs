using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Controls
{
    public class SectionTitle : Control
    {
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SectionTitle), null);
        public SectionTitle()
        {
            this.DefaultStyleKey = typeof(SectionTitle);
        }
    }
}
