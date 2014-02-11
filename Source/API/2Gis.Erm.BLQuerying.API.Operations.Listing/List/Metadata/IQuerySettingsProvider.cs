using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public interface IQuerySettingsProvider
    {
        QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel);
    }
}