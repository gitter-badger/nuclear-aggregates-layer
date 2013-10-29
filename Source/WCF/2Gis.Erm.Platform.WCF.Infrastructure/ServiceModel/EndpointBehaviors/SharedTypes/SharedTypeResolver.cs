using System;
using System.Runtime.Serialization;
using System.Xml;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public sealed class SharedTypeResolver : DataContractResolver
    {
        private readonly XmlDictionary _xmlDictionary = new XmlDictionary();
        private readonly SoapTypeNameConveter _typeNameConveter;

        public SharedTypeResolver(SoapTypeNameConveter typeNameConveter)
        {
            _typeNameConveter = typeNameConveter;
        }

        public override bool TryResolveType(Type type,
                                            Type declaredType,
                                            DataContractResolver knownTypeResolver,
                                            out XmlDictionaryString typeName,
                                            out XmlDictionaryString typeNamespace)
        {
            // resolve types by type if namespace starts with DoubleGis
            if (type.Namespace != null && type.Namespace.StartsWith("DoubleGis"))
            {
                var soapName = _typeNameConveter.ConvertToSoapName(type);

                typeName = _xmlDictionary.Add(soapName.Item1);
                typeNamespace = _xmlDictionary.Add(soapName.Item2);
                return true;
            }

            return knownTypeResolver.TryResolveType(type, declaredType, knownTypeResolver, out typeName, out typeNamespace);
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            // resolve names if xml namespace is 2Gis xml namespace
            var type = string.Equals(typeNamespace, SoapTypeNameConveter.DataContractsNamespace, StringComparison.OrdinalIgnoreCase)
                           ? knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver)
                           : _typeNameConveter.ConvertToClrType(Tuple.Create(typeName, typeNamespace));
            return type;
        }
    }
}
