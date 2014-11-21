using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Basic
{
    /// <summary>
    /// Interaction logic for EnumSelectionComboBox.xaml
    /// </summary>
    public partial class EnumSelectionComboBox : ComboBox
    {
        private Type _enumType;

        public EnumSelectionComboBox()
        {
            InitializeComponent();
            IsEditable = false;
            Loaded += (s, e) => InitEnumType();
            DataContextChanged += (s, e) => InitEnumType();
        }

        public Type SourceEnumType
        {
            get
            {
                return _enumType;
            }
            set
            {
                _enumType = value;
                FillComboBox();
            }
        }

        private void InitEnumType()
        {
            // Если SourceEnumType не указан явно, то пытаемся его определить из биндинга свойства SelectedValue.
            if (_enumType == null)
            {
                var binding = GetBindingExpression(SelectedValueProperty);
                if (binding != null && binding.ResolvedSourcePropertyName != null && binding.ResolvedSource != null)
                {
                    var property = TypeDescriptor.GetProperties(binding.ResolvedSource).Find(binding.ResolvedSourcePropertyName, false);
                    if (property != null)
                    {
                        SourceEnumType = property.PropertyType;
                    }
                }
            }
        }

        private void FillComboBox()
        {
            if (_enumType != null)
            {
                if (_enumType.IsEnum)
                {
                    ItemsSource = Enum.GetValues(_enumType);
                }
                else 
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
