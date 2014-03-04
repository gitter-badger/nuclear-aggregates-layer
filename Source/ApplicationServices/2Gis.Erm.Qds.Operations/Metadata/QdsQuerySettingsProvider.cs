using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public sealed class QdsQuerySettingsProvider : IQuerySettingsProvider
    {
        public QuerySettings GetQuerySettings(EntityName entityName, Type documentType, SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                SortOrder = searchListModel.Sort,
                SortDirection = searchListModel.Dir,
                DefaultFilter = null,
                UserInputFilter = searchListModel.FilterInput,
            };

            return querySettings;
        }
    }
}
