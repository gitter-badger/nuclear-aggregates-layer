using System;
using System.IO;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public sealed class SoapTypeNameConveter
    {
        internal const string DataContractsNamespace = "http://2gis.ru/erm/api/datacontracts/";
        private static readonly char PathSeparator = Path.AltDirectorySeparatorChar;

        public string GetClrNamespace(string soapNamespace)
        {
            return soapNamespace.Substring(soapNamespace.LastIndexOf(PathSeparator) + 1);
        }

        public string GetSoapNamespace(string clrNamespace, string assemblyFullName)
        {
            var assemblyName = assemblyFullName.Replace(" ", string.Empty).Replace(',', PathSeparator);
            return DataContractsNamespace + assemblyName + PathSeparator + clrNamespace;
        }

        public Tuple<string, string> ConvertToSoapName(Type type)
        {
            var ns = GetSoapNamespace(type.Namespace, type.Assembly.FullName);
            return Tuple.Create(type.FullName, ns);
        }

        public Type ConvertToClrType(Tuple<string, string> soapName)
        {
            var ns = soapName.Item2.Replace(DataContractsNamespace, string.Empty);
            var assemblyName = ns.Substring(0, ns.LastIndexOf(PathSeparator)).Replace(PathSeparator, ',');
            return Type.GetType(string.Format("{0}, {1}", soapName.Item1, assemblyName));
        }
    }
}