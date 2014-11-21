using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls
{
    public class NamedEnumComboBox : NamedComboBox
    {
        private Type _enumType;

        static NamedEnumComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NamedEnumComboBox), new FrameworkPropertyMetadata(typeof(NamedEnumComboBox)));
            IsTabStopProperty.OverrideMetadata(typeof(NamedEnumComboBox), new FrameworkPropertyMetadata(false));
        }

        public NamedEnumComboBox()
        {
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
                    else
                    {
                        var customTypeProvider = binding.ResolvedSource as ICustomTypeProvider;
                        if (customTypeProvider != null)
                        {
                            var dynamicProperty = customTypeProvider.GetCustomType().GetProperties().FirstOrDefault(x => x.Name == binding.ResolvedSourcePropertyName);
                            if (dynamicProperty != null)
                            {
                                SourceEnumType = dynamicProperty.PropertyType;
                            }
                        }
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