using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public sealed class QdsQuerySettingsProvider : IQuerySettingsProvider
    {
        public QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                SortOrder = searchListModel.Sort,
                SortDirection = searchListModel.Dir,
                FilterPredicate = null,
                Fields = null,
                DefaultFilter = null,
                UserInputFilter = searchListModel.FilterInput,
                MainAttribute = null,
            };

            return querySettings;
        }
    }
}
