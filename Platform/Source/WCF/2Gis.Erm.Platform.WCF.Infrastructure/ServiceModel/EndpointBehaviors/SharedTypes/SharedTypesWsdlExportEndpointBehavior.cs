using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

using Message = System.Web.Services.Description.Message;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public class SharedTypesWsdlExportEndpointBehavior : IEndpointBehavior, IWsdlExportExtension
    {
        private static readonly Func<SoapTypeNameConveter, Type, XmlQualifiedName> XmlQualifiedNameClrCreationStrategy =
            (converter, type) =>
            {
                var soapName = converter.ConvertToSoapName(type);
                return new XmlQualifiedName(soapName.Item1, soapName.Item2);
            };

        private static readonly Func<XmlQualifiedName, string, XmlQualifiedName> XmlQualifiedNameMixedCreationStrategy =
            (name, soapNamespace) => new XmlQualifiedName(name.Name, soapNamespace);

        private readonly HashSet<Type> _sharedTypes;
        private readonly SoapTypeNameConveter _typeNameConveter;
        private readonly IDictionary<string, string> _namespacesByAssemblies;
        private readonly DataContractResolver _sharedTypeResolver;

        public SharedTypesWsdlExportEndpointBehavior(HashSet<Type> sharedTypes,
                                                     SoapTypeNameConveter typeNameConveter,
                                                     IDictionary<string, string> namespacesByAssemblies,
                                                     DataContractResolver sharedTypeResolver)
        {
            _sharedTypes = sharedTypes;
            _typeNameConveter = typeNameConveter;
            _namespacesByAssemblies = namespacesByAssemblies;
            _sharedTypeResolver = sharedTypeResolver;
        }

        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            var dataContractExporter = ExportSharedTypes(exporter);
            TransformXmlSchemas(dataContractExporter.Schemas.Schemas().OfType<XmlSchema>());
            TransformXmlSchemaElements(dataContractExporter.Schemas.GlobalElements.Values.OfType<XmlSchemaElement>());
            TransformXmlSchemaSimpleTypes(dataContractExporter.Schemas.GlobalTypes.Values.OfType<XmlSchemaSimpleType>());
            TransformXmlSchemaComplexTypes(dataContractExporter.Schemas.GlobalTypes.Values.OfType<XmlSchemaComplexType>());
            TransformServiceMessages(exporter.GeneratedWsdlDocuments.OfType<ServiceDescription>().SelectMany(x => x.Messages.OfType<Message>()));
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            AttachSharedTypeResolver(endpoint.Contract.Operations);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            AttachSharedTypeResolver(endpoint.Contract.Operations);
        }

        private void AttachSharedTypeResolver(IEnumerable<OperationDescription> operationDescriptions)
        {
            foreach (var operationDescription in operationDescriptions)
            {
                var serializerBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>();

                if (serializerBehavior == null)
                {
                    serializerBehavior = new DataContractSerializerOperationBehavior(operationDescription);
                    operationDescription.Behaviors.Add(serializerBehavior);
                }

                serializerBehavior.DataContractResolver = _sharedTypeResolver;
            }
        }

        private XsdDataContractExporter ExportSharedTypes(WsdlExporter exporter)
        {
            object defaultDataContractExporter;
            XsdDataContractExporter dataContractExporter;
            if (!exporter.State.TryGetValue(typeof(XsdDataContractExporter), out defaultDataContractExporter))
            {
                dataContractExporter = new XsdDataContractExporter(exporter.GeneratedXmlSchemas);
                exporter.State.Add(typeof(XsdDataContractExporter), dataContractExporter);
            }
            else
            {
                dataContractExporter = (XsdDataContractExporter)defaultDataContractExporter;
            }

            if (dataContractExporter.CanExport(_sharedTypes))
            {
                dataContractExporter.Export(_sharedTypes);
            }

            return dataContractExporter;
        }

        private void TransformXmlSchemas(IEnumerable<XmlSchema> xmlSchemas)
        {
            foreach (var schema in xmlSchemas)
            {
                var clrNamespace = _typeNameConveter.GetClrNamespace(schema.TargetNamespace);
                var targetNamespaceAssemblyName = _namespacesByAssemblies.Where(x => !string.IsNullOrEmpty(x.Key) &&
                                                                                     clrNamespace.Contains(x.Key))
                    .Select(x => x.Value)
                    .FirstOrDefault();
                if (targetNamespaceAssemblyName != null)
                {
                    schema.TargetNamespace = _typeNameConveter.GetSoapNamespace(clrNamespace, targetNamespaceAssemblyName);
                }

                //var xmlSerializerNamespaces = new XmlSerializerNamespaces();
                //foreach (var xmlSerializerNamespace in schema.Namespaces.ToArray())
                //{
                //    if (xmlSerializerNamespace.Name == "tns")
                //    {
                //        var assembly = _namespacesByAssemblies.Where(x => !string.IsNullOrEmpty(x.Key) &&
                //                                                          xmlSerializerNamespace.Namespace.Contains(x.Key))
                //            .Select(x => x.Value.FirstOrDefault())
                //            .FirstOrDefault();
                //        if (assembly != null)
                //        {
                //            xmlSerializerNamespaces.Add(xmlSerializerNamespace.Name, assembly.FullName);
                //            continue;
                //        }

                //        xmlSerializerNamespaces.Add(xmlSerializerNamespace.Name, xmlSerializerNamespace.Namespace);
                //    }
                //}

                //schema.Namespaces = xmlSerializerNamespaces;
            }
        }

        private void TransformXmlSchemaElements(IEnumerable<XmlSchemaElement> schemaElements)
        {
            foreach (var schemaElement in schemaElements)
            {
                var complexType = schemaElement.SchemaType as XmlSchemaComplexType;
                if (complexType != null)
                {
                    var sequence = complexType.ContentTypeParticle as XmlSchemaSequence;
                    if (sequence != null)
                    {
                        foreach (var element in sequence.Items.OfType<XmlSchemaElement>())
                        {
                            TransformSequenceElement(element);
                        }
                    }
                }

                var clrType = _sharedTypes.FirstOrDefault(x => schemaElement.SchemaTypeName.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (clrType != null)
                {
                    schemaElement.Name = clrType.FullName;
                    schemaElement.SchemaTypeName = XmlQualifiedNameClrCreationStrategy(_typeNameConveter, clrType);
                }
                else
                {
                    var clrNamespace = _typeNameConveter.GetClrNamespace(schemaElement.SchemaTypeName.Namespace);
                    var key = _namespacesByAssemblies.Keys.FirstOrDefault(x => clrNamespace.Contains(x));
                    if (key != null)
                    {
                        var assemblyName = _namespacesByAssemblies[key];
                        if (assemblyName != null)
                        {
                            schemaElement.SchemaTypeName = XmlQualifiedNameMixedCreationStrategy(schemaElement.SchemaTypeName, 
                                                                                                 _typeNameConveter.GetSoapNamespace(clrNamespace, assemblyName));
                        }
                    }
                }
            }
        }

        private void TransformSequenceElement(XmlSchemaElement element)
        {
            if (element.SchemaTypeName.Name.StartsWith("ArrayOf"))
            {
                var clrNamespace = _typeNameConveter.GetClrNamespace(element.SchemaTypeName.Namespace);
                var schemaTypeAssemblyName = _namespacesByAssemblies.Where(x => !string.IsNullOrEmpty(x.Key) &&
                                                                                clrNamespace.Contains(x.Key))
                                                                    .Select(x => x.Value)
                                                                    .FirstOrDefault();
                if (schemaTypeAssemblyName != null)
                {
                    element.SchemaTypeName = XmlQualifiedNameMixedCreationStrategy(element.SchemaTypeName,
                                                                                   _typeNameConveter.GetSoapNamespace(clrNamespace, schemaTypeAssemblyName));
                }
            }
            else
            {
                var elementClrType = _sharedTypes.FirstOrDefault(x => element.SchemaTypeName.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (elementClrType != null)
                {
                    element.SchemaTypeName = XmlQualifiedNameClrCreationStrategy(_typeNameConveter, elementClrType);
                }
            }
        }

        private void TransformXmlSchemaSimpleTypes(IEnumerable<XmlSchemaSimpleType> simpleTypes)
        {
            foreach (var simpleType in simpleTypes)
            {
                var clrType = _sharedTypes.FirstOrDefault(x => simpleType.QualifiedName.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (clrType != null)
                {
                    simpleType.Name = clrType.FullName;
                }
            }
        }

        private void TransformXmlSchemaComplexTypes(IEnumerable<XmlSchemaComplexType> complexTypes)
        {
            foreach (var complexType in complexTypes)
            {
                var sequence = complexType.ContentTypeParticle as XmlSchemaSequence;
                if (sequence != null)
                {
                    foreach (var element in sequence.Items.OfType<XmlSchemaElement>())
                    {
                        TransformSequenceElement(element);
                    }
                }

                if (complexType.ContentModel != null)
                {
                    var contentExtension = complexType.ContentModel.Content as XmlSchemaComplexContentExtension;
                    if (contentExtension != null)
                    {
                        var contentExtensionBaseClrType =
                            _sharedTypes.FirstOrDefault(x => contentExtension.BaseTypeName.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                        if (contentExtensionBaseClrType != null)
                        {
                            contentExtension.BaseTypeName = XmlQualifiedNameClrCreationStrategy(_typeNameConveter, contentExtensionBaseClrType);
                        }
                    }
                }

                var complexClrType = _sharedTypes.FirstOrDefault(x => complexType.QualifiedName.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (complexClrType != null)
                {
                    complexType.Name = complexClrType.FullName;
                }
            }
        }

        private void TransformServiceMessages(IEnumerable<Message> messages)
        {
            foreach (var messagePart in messages.SelectMany(x => x.Parts.OfType<MessagePart>()))
            {
                var messagePartType = _sharedTypes.FirstOrDefault(x => messagePart.Element.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (messagePartType != null)
                {
                    messagePart.Element = XmlQualifiedNameMixedCreationStrategy(messagePart.Element, messagePartType.Assembly.FullName);
                    messagePart.Type = XmlQualifiedNameClrCreationStrategy(_typeNameConveter, messagePartType);
                }
            }
        }
    }
}