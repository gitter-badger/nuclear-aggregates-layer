using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    // FIXME {m.pashuk, 29.04.2014}: Филипп планировал использовать эту сборку как техническую для доступка к Elasic через нест. Т.е. тут не должно быть бизнеса. Надо обсудить.
    // FIXME {m.pashuk, 29.04.2014}: Нестыковка с названием файла
    public static class IndexMappingMetadata
    {
        public static readonly List<Tuple<Type, string>> DocTypeToIndexNameMap = new List<Tuple<Type, string>>
        {
            Tuple.Create(typeof(MigrationDoc), "Metadata"),
            Tuple.Create(typeof(ReplicationQueue), "Metadata"),

            Tuple.Create(typeof(UserDoc), "Metadata"),
            Tuple.Create(typeof(TerritoryDoc), "Metadata"),
            Tuple.Create(typeof(RecordIdState), "Metadata"),

            Tuple.Create(typeof(ClientGridDoc), "Data"),
            Tuple.Create(typeof(OrderGridDoc), "Data"),
            Tuple.Create(typeof(FirmGridDoc), "Data"),
        };

        public static Type GetDocumentType(string documentTypeName)
        {
            var documentType = DocTypeToIndexNameMap.Where(x => string.Equals(x.Item1.Name, documentTypeName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Item1).Single();
            return documentType;
        }
    }
}