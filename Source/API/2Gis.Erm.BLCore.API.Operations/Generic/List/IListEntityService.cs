using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IListEntityService : IOperation<ListIdentity>
    {
        ListResult List(SearchListModel searchListModel);
    }

    // TODO: entityName -> SearchListModel?
    public interface IListNonGenericEntityService : IOperation<ListNonGenericIdentity>
    {
        ListResult List(EntityName entityName, SearchListModel searchListModel);
    }
}