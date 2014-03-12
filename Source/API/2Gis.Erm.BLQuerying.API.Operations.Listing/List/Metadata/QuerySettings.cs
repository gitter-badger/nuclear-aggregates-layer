using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public sealed class QuerySettings
    {
        public EntityName EntityName { get; set; }

        public int SkipCount { get; set; }
        public int TakeCount { get; set; }
        public string SortOrder { get; set; }
        public string SortDirection { get; set; }

        public string DefaultFilter { get; set; }
        public string UserInputFilter { get; set; }

        public IReadOnlyDictionary<string, string> ExtendedInfoMap { get; set; }

        // TODO убрать в extended info
        public EntityName ParentEntityName { get; set; }
        public long? ParentEntityId { get; set; }
    }
}