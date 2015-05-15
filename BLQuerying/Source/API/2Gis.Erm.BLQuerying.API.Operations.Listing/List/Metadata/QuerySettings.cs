using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public sealed class QuerySettings
    {
        public int SkipCount { get; set; }
        public int TakeCount { get; set; }
        public IReadOnlyCollection<QuerySettingsSort> Sort { get; set; }

        public string FilterName { get; set; }

        public string UserInputFilter { get; set; }

        public IReadOnlyDictionary<string, string> ExtendedInfoMap { get; set; }

        public IEntityType ParentEntityName { get; set; }
        public long? ParentEntityId { get; set; }
        public SearchListModel SearchListModel { get; set; }
    }

    public enum SortDirection
    {
        Ascending,
        Descending,
    }

    public sealed class QuerySettingsSort
    {
        public string PropertyName { get; set; }
        public SortDirection Direction { get; set; }
    }
}