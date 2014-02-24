using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public sealed class QuerySettingsProviderSelector : IQuerySettingsProvider
    {
        private readonly IQuerySettingsProvider _oldProvider;
        private readonly IQuerySettingsProvider _newProvider;

        public QuerySettingsProviderSelector(IQuerySettingsProvider oldProvider, IQuerySettingsProvider newProvider)
        {
            _oldProvider = oldProvider;
            _newProvider = newProvider;
        }

        public QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel)
        {
            switch (entityName)
            {
                case EntityName.Client:
                    return _newProvider.GetQuerySettings(entityName, searchListModel);
                default:
                    return _oldProvider.GetQuerySettings(entityName, searchListModel);
            }
        }
    }
}
