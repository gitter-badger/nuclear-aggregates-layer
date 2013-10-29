using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public class SharedTypesMessageInspector : IDispatchMessageInspector
    {
        private const string DoubleGisNamespaceMarker = "DoubleGis";
        private const string XmlNamespace = "xmlns";
        
        private const string StandartDataContractsNamespace = "http://schemas.datacontract.org/2004/07/";
        private const string SoapEnvelopeNamespace = "http://www.w3.org/2003/05/soap-envelope";

        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private readonly SoapTypeNameConveter _typeNameConveter;
        private readonly IDictionary<string, string> _namespacesByAssemblies;

        public SharedTypesMessageInspector(SoapTypeNameConveter typeNameConveter, IDictionary<string, string> namespacesByAssemblies)
        {
            _typeNameConveter = typeNameConveter;
            _namespacesByAssemblies = namespacesByAssemblies;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            request = ModifyMessage(request, ReplaceNamespacesAfterReceive);
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            reply = ModifyMessage(reply, ReplaceNamespacesBeforeSend);
        }

        private static Message ModifyMessage(Message originalMessage, Func<MemoryStream, MemoryStream> namespaceReplacer)
        {
            var memoryStream = new MemoryStream();

            var writer = XmlDictionaryWriter.CreateTextWriter(memoryStream);
            originalMessage.WriteMessage(writer);
            writer.Flush();

            memoryStream = namespaceReplacer(memoryStream);

            var reader = XmlReader.Create(memoryStream);
            return Message.CreateMessage(reader, int.MaxValue, originalMessage.Version);
        }

        private MemoryStream ReplaceNamespacesBeforeSend(MemoryStream stream)
        {
            var xml = DefaultEncoding.GetString(stream.ToArray());
            
            var regex = new Regex(string.Format("{0}[\\w\\.]+", StandartDataContractsNamespace));
            xml = regex.Replace(xml,
                x =>
                {
                    string assemblyFullName;
                    var clrNamespace = _typeNameConveter.GetClrNamespace(x.Value);

                    return _namespacesByAssemblies.TryGetValue(clrNamespace, out assemblyFullName) 
                        ? _typeNameConveter.GetSoapNamespace(clrNamespace, assemblyFullName) 
                        : x.Value;
                });

            return new MemoryStream(DefaultEncoding.GetBytes(xml));
        }

        private MemoryStream ReplaceNamespacesAfterReceive(MemoryStream stream)
        {
            stream.Position = 0;
            var envelope = XElement.Load(stream);

            var ns = XNamespace.Get(SoapEnvelopeNamespace);
            var body = envelope.Elements(ns + "Body").SingleOrDefault();
            if (body != null)
            {
                var elementsToReplace = body.Descendants()
                    .Where(x => !x.Attributes().Any(y => y.IsNamespaceDeclaration) &&
                                !string.IsNullOrEmpty(x.GetPrefixOfNamespace(x.Name.Namespace)))
                    .Select(x =>
                    {
                        var soapNamespace = x.Name.NamespaceName;
                        var clrNamespace = _typeNameConveter.GetClrNamespace(soapNamespace);

                        ns = XNamespace.Get(StandartDataContractsNamespace + clrNamespace);
                        var replacement = x.HasElements ? new XElement(ns + x.Name.LocalName, x.Descendants()) : new XElement(ns + x.Name.LocalName, x.Value);
                        replacement.Add(x.Attributes());
                        return Tuple.Create(x, replacement);
                    })
                    .ToArray();
                foreach (var tuple in elementsToReplace)
                {
                    tuple.Item1.ReplaceWith(tuple.Item2);
                }
            }

            var regex = new Regex(string.Format("{0}=\"[\\w\\d\\:./=]+\"", XmlNamespace));
            var xml = regex.Replace(envelope.ToString(),
                x =>
                {
                    var soapNamespace = x.Value.Replace(string.Format("{0}=", XmlNamespace), string.Empty).Trim('"');
                    return soapNamespace.Contains(DoubleGisNamespaceMarker)
                        ? string.Format("{0}=\"{1}{2}\"", XmlNamespace, StandartDataContractsNamespace, _typeNameConveter.GetClrNamespace(soapNamespace))
                        : x.Value;
                });


            return new MemoryStream(DefaultEncoding.GetBytes(xml));
        }
    }
}