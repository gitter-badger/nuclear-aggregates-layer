using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Basic
{
    /// <summary>
    /// Контрол для редактирования значения какого-либо свойства объекта.
    /// Может отображать либо один из простейших предопределенных шаблонов 
    /// (требуется указать значения для свойств ControlType и Binding), 
    /// либо явно указанный шаблон (требуется указать значение свойства CustomContentTemplate).
    /// </summary>
    public sealed partial class FieldEditControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(FieldEditControl));

        public static readonly DependencyProperty ControlTypeProperty =
            DependencyProperty.Register("ControlType", typeof(FieldControlType), typeof(FieldEditControl));

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register("IsRequired", typeof(bool), typeof(FieldEditControl), new PropertyMetadata(default(bool)));

        private MarkupExtension _binding;
        private DataTemplate _customContentTemplate;
        

        public FieldEditControl()
        { 
            InitializeComponent();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Привязка к свойству, которая будет передана в шаблон.
        /// </summary>
        public MarkupExtension Binding
        {
            get
            {
                return _binding;
            }
            set
            {
                if (_binding != value)
                {
                    _binding = value;
                    OnPropertyChanged();
                }
            }
        }

        public FieldControlType ControlType
        {
            get { return (FieldControlType)GetValue(ControlTypeProperty); }
            set { SetValue(ControlTypeProperty, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public DataTemplate CustomContentTemplate
        {
            get
            {
                return _customContentTemplate;
            }
            set
            {
                if (_customContentTemplate != value)
                {
                    _customContentTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }   
            set { SetValue(IsRequiredProperty, value); }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
