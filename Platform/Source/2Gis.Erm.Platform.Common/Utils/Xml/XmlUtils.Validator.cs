using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace DoubleGis.Erm.Platform.Common.Utils.Xml
{
    public static partial class XmlUtils
    {
        public static bool ValidateXml(this string xml, string xsd, out string error)
        {
            using (var stringReader = new StringReader(xml))
            {
                return ValidateXml(stringReader, xsd, out error);
            }
        }

        private static bool ValidateXml(this TextReader textReader, string xsd, out string error)
        {
            var xmlSchemaSet = CreateXmlSchemaSetForXsd(xsd);
            var xmlReaderSettings = new XmlReaderSettings
                                        {
                                            ValidationType = ValidationType.Schema,
                                            Schemas = xmlSchemaSet,
                                            ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                                              XmlSchemaValidationFlags.AllowXmlAttributes |
                                                              XmlSchemaValidationFlags.ReportValidationWarnings
                                        };

            var stringBuilder = new StringBuilder();
            ValidateXml(textReader, xmlReaderSettings, (sender, eventArgs) => stringBuilder.AppendLine(eventArgs.Message));
            error = stringBuilder.ToString();

            return stringBuilder.Length == 0;
        }

        private static XmlSchemaSet CreateXmlSchemaSetForXsd(string xsd)
        {
            using (var stringReader = new StringReader(xsd))
            {
                var xmlSchema = XmlSchema.Read(stringReader, null);

                var xmlSchemaSet = new XmlSchemaSet();
                xmlSchemaSet.Add(xmlSchema);
                xmlSchemaSet.Compile();

                return xmlSchemaSet;
            }
        }

        private static void ValidateXml(TextReader textReader, XmlReaderSettings xmlReaderSettings, ValidationEventHandler validationEventHandler)
        {
            try
            {
                xmlReaderSettings.ValidationEventHandler += validationEventHandler;

                using (var xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
                {
                    while (xmlReader.Read())
                    {
                    }
                }
            }
            finally
            {
                xmlReaderSettings.ValidationEventHandler -= validationEventHandler;
            }
        }
    }
}