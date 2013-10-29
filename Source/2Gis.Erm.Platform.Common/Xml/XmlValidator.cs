using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DoubleGis.Erm.Platform.Common.Xml
{
    public static class XmlValidator
    {
        public static XmlSchemaSet CreateXmlSchemaSetForXsd(string xsd)
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

        public static bool Validate(XDocument document, XmlSchemaSet xmlSchemaSet, out string error)
        {
            var stringBuilder = new StringBuilder();
            document.Validate(xmlSchemaSet, (sender, eventArgs) => stringBuilder.AppendLine(eventArgs.Message));
            error = stringBuilder.ToString();

            return stringBuilder.Length == 0;
        }

        public static bool Validate(string xml, XmlSchemaSet xmlSchemaSet, out string error)
        {
            using (var stringReader = new StringReader(xml))
            {
                return Validate(stringReader, xmlSchemaSet, out error);
            }
        }

        public static bool Validate(StreamReader streamReader, XmlSchemaSet xmlSchemaSet, out string error)
        {
            return Validate((TextReader)streamReader, xmlSchemaSet, out error);
        }

        public static bool Validate(TextReader textReader, XmlSchemaSet xmlSchemaSet, out string error)
        {
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