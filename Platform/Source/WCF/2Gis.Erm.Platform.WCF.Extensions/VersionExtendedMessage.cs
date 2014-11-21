using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public sealed class VersionExtendedMessage : Message
    {
        private static readonly XName ProbeMatchesCd1 =
            XName.Get(ProtocolStrings.SchemaNames.ProbeMatchElement, ProtocolStrings.VersionCd1.Namespace);
        private static readonly XName ProbeMatches11 =
            XName.Get(ProtocolStrings.SchemaNames.ProbeMatchElement, ProtocolStrings.Version11.Namespace);
        private static readonly XName ProbeMatchesApril2005 =
            XName.Get(ProtocolStrings.SchemaNames.ProbeMatchElement, ProtocolStrings.VersionApril2005.Namespace);

        private static readonly XName TypesCd1 =
            XName.Get(ProtocolStrings.SchemaNames.TypesElement, ProtocolStrings.VersionCd1.Namespace);
        private static readonly XName Types11 =
            XName.Get(ProtocolStrings.SchemaNames.TypesElement, ProtocolStrings.Version11.Namespace);
        private static readonly XName TypesApril2005 =
            XName.Get(ProtocolStrings.SchemaNames.TypesElement, ProtocolStrings.VersionApril2005.Namespace);

        private readonly XElement _body;
        private readonly Message _baseMessage;

        public VersionExtendedMessage(Message baseMessage)
        {
            _baseMessage = baseMessage;
            _body = ParseMessage(baseMessage);
        }

        public override MessageHeaders Headers
        {
            get { return _baseMessage.Headers; }
        }

        public override MessageProperties Properties
        {
            get { return _baseMessage.Properties; }
        }

        public override MessageVersion Version
        {
            get { return _baseMessage.Version; }
        }

        public void InjectEndpointVersions(IDictionary<Tuple<Uri, XName>, Version> serviceVersions, string adaptation)
        {
            var probeMatches = _body.Elements().Where(IsProbeMatchElement);

            foreach (var probeMatch in probeMatches)
            {
                var address = ParseAddress(probeMatch);
                var types = ParseTypes(probeMatch.Elements().Where(IsTypesElement));
                var version = types.Select(name => GetTypeVersionElement(name, address, serviceVersions))
                                   .FirstOrDefault(element => element != null);

                if (version != null)
                {
                    probeMatch.Add(version);
                }

                var adaptationNode = CreateAdaptationNode(adaptation);
                if (adaptationNode != null)
                {
                    probeMatch.Add(adaptationNode);
                }
            }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            _body.WriteTo(writer);
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            OnWriteBodyContents(writer);
        }

        private static Uri ParseAddress(XElement probeMatch)
        {
            return probeMatch.Elements()
                      .Where(element => element.Name.LocalName == "EndpointReference")
                      .SelectMany(element => element.Elements())
                      .Where(element => element.Name.LocalName == "Address")
                      .Select(element => new Uri(element.Value))
                      .FirstOrDefault();
        }

        private static IEnumerable<XName> ParseTypes(IEnumerable<XElement> elements)
        {
            var result = new List<XName>();
            foreach (var element in elements)
            {
                var types = element.Value.Split(' ');
                foreach (var type in types)
                {
                    var typeNameItems = type.Split(':');
                    var ns = typeNameItems[0];
                    var name = typeNameItems[1];

                    var attribute = element.Attribute(XName.Get(ns, XNamespace.Xmlns.NamespaceName));
                    if (attribute != null)
                    {
                        ns = attribute.Value;
                    }

                    result.Add(XName.Get(name, ns));
                }
            }

            return result;
        }

        private static bool IsProbeMatchElement(XElement element)
        {
            return element.Name == ProbeMatchesCd1 ||
                   element.Name == ProbeMatches11 ||
                   element.Name == ProbeMatchesApril2005;
        }

        private static bool IsTypesElement(XElement element)
        {
            return element.Name == TypesCd1 ||
                   element.Name == Types11 ||
                   element.Name == TypesApril2005;
        }

        private static XElement ParseMessage(Message baseMessage)
        {
            using (var stream = new MemoryStream())
            {
                var writerSettings = new XmlWriterSettings { CloseOutput = false };
                using (var xmlWriter = XmlWriter.Create(stream, writerSettings))
                using (var xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter))
                {
                    baseMessage.WriteBodyContents(xmlDictionaryWriter);
                }

                stream.Position = 0;
                return XElement.Load(stream);
            }
        }

        private static XElement CreateAdaptationNode(string adaptation)
        {
            if (string.IsNullOrWhiteSpace(adaptation))
            {
                return null;
            }

            var result = new XElement("BusinessLogicAdaptation");
            result.SetValue(adaptation);
            return result;
        }

        private static XElement GetTypeVersionElement(XName typeName, Uri address, IDictionary<Tuple<Uri, XName>, Version> serviceVersions)
        {
            Version version;
            var key = Tuple.Create(address, typeName);
            if (serviceVersions.TryGetValue(key, out version))
            {
                var result = new XElement("ServiceVersion");
                result.SetAttributeValue("TypeName", typeName);
                result.SetValue(version);
                return result;
            }

            return null;
        }
    }
}
