using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources
{
    public class TemplateDescriptor : ITemplateDescriptor
    {
        public TemplateDescriptor(IResourceDescriptor template, params IResourceDescriptor[] templateParameters)
        {
            TemplateParameters = templateParameters;
            Template = template;
        }

        public IResourceDescriptor Template { get; private set; }
        public IEnumerable<IResourceDescriptor> TemplateParameters { get; private set; }
        public bool TryFormat(CultureInfo culture, out string result, params object[] templateParameterValueContainers)
        {
            result = string.Empty;
            var parameterValues = new List<string>();

            var stringTemplateDescriptor = Template as IStringResourceDescriptor;
            var templateString = stringTemplateDescriptor != null ? stringTemplateDescriptor.GetValue(culture) : Template.ToString();

            foreach (var templateParameter in TemplateParameters)
            {
                var propertyDescriptor = templateParameter as IPropertyDescriptor;
                if (propertyDescriptor != null)
                {
                    var propertyValueFound = false;
                    foreach (var valueContainer in templateParameterValueContainers)
                    {
                        object propertyValue;
                        if (propertyDescriptor.TryGetValue(valueContainer, out propertyValue))
                        {
                            parameterValues.Add(propertyValue != null ? propertyValue.ToString() : string.Empty);
                            propertyValueFound = true;
                            break;
                        }
                    }

                    if (!propertyValueFound)
                    {
                        return false;
                    }
                }
                else
                {
                    var stringDescriptor = templateParameter as IStringResourceDescriptor;
                    parameterValues.Add(stringDescriptor != null ? stringDescriptor.GetValue(culture) : templateParameter.ToString());
                }
            }

            try
            {
                result = string.Format(templateString, parameterValues.ToArray());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public string ResourceKeyToString()
        {
            return string.Format(Template.ToString(), TemplateParameters.Select(x => string.Format("{0}", x.ResourceKeyToString())));
        }

        public override string ToString()
        {
            return ResourceKeyToString();
        }
    }
}
