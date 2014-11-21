using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using DoubleGis.Platform.UI.WPF.Infrastructure.Util.FocusableBinding;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Basic
{
    public class FieldEditControlTypeToTemplateConverter : IMultiValueConverter
    {
        public FieldEditControlTypeToTemplateConverter()
        {
            Templates = new List<FieldControlTemplate>();
        }

        public List<FieldControlTemplate> Templates { get; set; }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Логика по определению шаблона для контрола FieldEditControl
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var fieldEditControl = values[0] as FieldEditControl;
            if (fieldEditControl == null)
            {
                return null;
            }

            if (fieldEditControl.ControlType == FieldControlType.Custom &&
                fieldEditControl.CustomContentTemplate == null)
            {
                throw new InvalidOperationException("CustomContentTemplate not specified.");
            }

            if (fieldEditControl.CustomContentTemplate != null)
            {
                return fieldEditControl.CustomContentTemplate;
            }

            string path = null;

            var bindingValue = values[2];
            if (bindingValue != null)
            {
                var binding = bindingValue as Binding;
                path = binding == null ? ((FocusableBinding)bindingValue).Path.Path : binding.Path.Path;
            }

            if (path == null)
            {
                return null;
            }

            var template = GetTemplate(fieldEditControl);
            if (template == null)
            {
                return null;
            }

            if (template.Resources["BindingPropertyPath"] == null)
            {
                throw new InvalidOperationException("Not found resource 'BindingPropertyPath'");
            }

            // Прокидываем название свойства из биндинга, указанного при в свойстве Binding 
            // контрола FieldEditControl в биндинг редактирующего контрола в выбранном шаблоне.
            // Передаем костылееобразно - через ресурсы шаблона (.
            template.Resources["BindingPropertyPath"] = new PropertyPath(path);
            return template;
        }

        private DataTemplate GetTemplate(FieldEditControl fieldEditControl)
        {
            var controlType = fieldEditControl.ControlType;
            if (controlType == FieldControlType.Auto)
            {
                // TODO : запилить автоопределение для шаблона.
            }

            var template = Templates.Where(x => x.Key == controlType).Select(x => x.Template).FirstOrDefault();
            if (template != null)
            {
                return template;
            }

            return null;
        }
    }
}
