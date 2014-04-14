using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DoubleGis.Erm.Platform.Common.Xml
{
    public static class XmlValidator
    {
        public static bool Validate(XDocument document, string xsd, out string error)
        {
            var stringBuilder = new StringBuilder();
            var xmlSchemaSet = CreateXmlSchemaSetForXsd(xsd);
            document.Validate(xmlSchemaSet, (sender, eventArgs) => stringBuilder.AppendLine(eventArgs.Message));
            error = stringBuilder.ToString();

            return stringBuilder.Length == 0;
        }

        public static bool Validate(string xml, string xsd, out string error)
        {
            using (var stringReader = new StringReader(xml))
            {
                return Validate(stringReader, xsd, out error);
            }
        }

        public static bool Validate(StreamReader streamReader, string xsd, out string error)
        {
            return Validate((TextReader)streamReader, xsd, out error);
        }

        public static bool Validate(TextReader textReader, string xsd, out string error)
        {
            var xmlSchemaSet = CreateXmlSchemaSetForXsd(xsd);
            var xmlReaderSettings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = xmlSchemaSet,
                };

            var stringBuilder = new StringBuilder();
            Validate(textReader, xmlReaderSettings, (sender, eventArgs) => stringBuilder.AppendLine(eventArgs.Message));
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

        private static void Validate(TextReader textReader, XmlReaderSettings xmlReaderSettings, ValidationEventHandler validationEventHandler)
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