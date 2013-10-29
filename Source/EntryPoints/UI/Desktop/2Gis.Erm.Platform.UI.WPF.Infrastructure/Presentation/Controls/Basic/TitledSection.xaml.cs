using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Basic
{
    public partial class TitledSection : Button
    {
        public TitledSection()
        {
            InitializeComponent();
            DefaultStyleKey = typeof(TitledSection);
        }

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

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitledSection), null);

        public Style TitleTextStyle
        {
            get
            {
                return (Style)GetValue(TitleTextStyleProperty);
            }
            set
            {
                SetValue(TitleTextStyleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleTextStyleProperty = DependencyProperty.Register("TitleTextStyle", typeof(Style), typeof(TitledSection));

        public Brush SectionLineBrush
        {
            get
            {
                return (Brush)GetValue(SectionLineBrushProperty);
            }
            set
            {
                SetValue(SectionLineBrushProperty, value);
            }
        }

        public static readonly DependencyProperty SectionLineBrushProperty = DependencyProperty.Register("SectionLineBrush", typeof(Brush), typeof(TitledSection));
    }
}
