using System;
using System.Xml.Linq;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Common.Utils.Xml
{
    public static partial class XmlUtils
    {
        public static bool TryGetAttributeValue<TValue>(
            this XElement element,
            string attributeName,
            out TValue attributeValue,
            out string report)
        {
            attributeValue = default(TValue);
            report = null;
            const string ReportTemplate = "Can't parse reguired attribute {0}. {1}";

            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                report = string.Format(ReportTemplate, attributeName, "Attribute not found");
                return false;
            }

            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                report = string.Format(ReportTemplate, attributeName, "Attribute value is empty");
                return false;
            }

            var targetType = typeof(TValue);

            try
            {
                if (typeof(IEntityType).IsAssignableFrom(targetType))
                {
                    attributeValue = (TValue)EntityType.Instance.Parse(int.Parse(attribute.Value));
                }
                else if (targetType.IsEnum)
                {
                    attributeValue = (TValue)Enum.Parse(targetType, attribute.Value);
                }
                else
                {
                    attributeValue = (TValue)Convert.ChangeType(attribute.Value, targetType);
                }
            }
            catch
            {
                report = string.Format(ReportTemplate, attributeName, "Can't convert attribute value " + attribute.Value + " to target type " + targetType.FullName);
                return false;
            }

            return true;
        }
    }
}
