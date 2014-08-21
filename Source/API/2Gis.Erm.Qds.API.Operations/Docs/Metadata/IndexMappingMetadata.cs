using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Qds.API.Operations.Docs.Metadata
{
    public static class IndexMappingMetadata
    {
        public static readonly List<Tuple<Type, string>> DocTypeToIndexNameMap = new List<Tuple<Type, string>>
        {
            Tuple.Create(typeof(MigrationDoc), "Metadata"),
            Tuple.Create(typeof(ReplicationQueue), "Metadata"),
            Tuple.Create(typeof(UserAuthorizationDoc), "Metadata"),
            Tuple.Create(typeof(TerritoryDoc), "Metadata"),

            Tuple.Create(typeof(ClientGridDoc), "Data"),
            Tuple.Create(typeof(OrderGridDoc), "Data"),
            Tuple.Create(typeof(FirmGridDoc), "Data"),
            Tuple.Create(typeof(UserGridDoc), "Data"),
            Tuple.Create(typeof(DepartmentGridDoc), "Data"),
            Tuple.Create(typeof(CurrencyGridDoc), "Data"),
            Tuple.Create(typeof(CountryGridDoc), "Data"),
            Tuple.Create(typeof(OrgUnitGridDoc), "Data"),
            Tuple.Create(typeof(LegalPersonGridDoc), "Data"),
            Tuple.Create(typeof(BargainGridDoc), "Data"),
        };

        public static Type GetDocumentType(string documentTypeName)
        {
            var documentType = DocTypeToIndexNameMap.Where(x => string.Equals(x.Item1.Name, documentTypeName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Item1).Single();
            return documentType;
        }

        public static Type GetIndexType(string indexName)
        {
            var indexType = DocTypeToIndexNameMap.Where(x => string.Equals(x.Item2, indexName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Item1).First();
            return indexType;
        }

        public static IEnumerable<Tuple<Type, string>> GetIndexTypes(IEnumerable<Type> documentTypes)
        {
            var indexTypes = DocTypeToIndexNameMap
                .Where(x => documentTypes.Contains(x.Item1))
                .GroupBy(x => x.Item2, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First());

            return indexTypes;
        }
    }
}