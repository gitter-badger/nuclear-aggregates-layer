using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class EntityDataListsContainer
    {
        public bool HasCard { get; set; }
        public string EntityName { get; set; }
        public IEnumerable<DataListStructure> DataViews { get; set; }
    }
}