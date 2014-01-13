using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Metadata
{
    public interface IQuerySettingsProvider
    {
        QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel);
    }
}