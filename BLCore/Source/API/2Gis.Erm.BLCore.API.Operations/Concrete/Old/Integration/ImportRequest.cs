using System.IO;
using System.Xml;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration
{
    public abstract class ImportRequest: Request
    {
        private readonly XmlReaderSettings _xmlReaderSettings = new XmlReaderSettings
        {
            // do not read unnecessary things
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,

            // do not validate xml schema
            ValidationType = ValidationType.None,

            // validate that we read well-formed xml document
            ConformanceLevel = ConformanceLevel.Document,
        };

        public Stream MessageStream { get; set; }
        public XmlReaderSettings XmlReaderSettings
        {
            get { return _xmlReaderSettings; }
        }
    }
}