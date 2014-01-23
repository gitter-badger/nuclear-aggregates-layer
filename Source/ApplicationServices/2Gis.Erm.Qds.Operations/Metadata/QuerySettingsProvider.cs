using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public sealed class QuerySettingsProvider : IQuerySettingsProvider
    {
        public QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                SortOrder = searchListModel.Sort,
                SortDirection = searchListModel.Dir,
                FilterPredicate = searchListModel.WhereExp,
                Fields = null,
                DefaultFilter = null,
                UserInputFilter = searchListModel.FilterInput,
                MainAttribute = null,
            };

            return querySettings;
        }
    }
}
