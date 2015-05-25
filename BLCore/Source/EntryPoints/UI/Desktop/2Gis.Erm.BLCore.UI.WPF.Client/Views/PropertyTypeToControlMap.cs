using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Views
{
    public static class PropertyTypeToControlMap
    {
        // TODO: Переделать на Expressions
        public static ControlDescriptor GetControlDescriptor(EntityPropertyMetadata propertyMetadata)
        {
            var type = propertyMetadata.Type;
            
            if (type.IsEnum)
            {
                return new ControlDescriptor
                    {
                        ControlName = typeof(NamedEnumComboBox).Name,
                        TargetPropertyName = "SelectedValue",
                        ReadOnlyExpression = "IsEnabled=\"False\""
                    };
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new ControlDescriptor
                    {
                        ControlName = typeof(NamedCheckBox).Name,
                        TargetPropertyName = "IsChecked",
                        ReadOnlyExpression = "IsEnabled=\"False\""
                    };
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new ControlDescriptor
                    {
                        ControlName = typeof(NamedDatePicker).Name,
                        TargetPropertyName = "SelectedDate",
                        ReadOnlyExpression = "IsEnabled=\"False\""
                    };
            }

            if (type == typeof(EntityReference))
            {
                return new ControlDescriptor
                    {
                        ControlName = typeof(NamedLookup).Name,
                        TargetPropertyName = "Reference",
                        ReadOnlyExpression = "IsReadOnly=\"True\""
                    };
            }

            return new ControlDescriptor
                {
                    ControlName = typeof(NamedTextBox).Name,
                    TargetPropertyName = "Text",
                    ReadOnlyExpression = "IsReadOnly=\"True\""
                };
        }
    }

    public sealed class ControlDescriptor
    {
        public string ControlName { get; set; }
        public string TargetPropertyName { get; set; }
        public string ReadOnlyExpression { get; set; }
    }
}