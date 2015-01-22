using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IListEntityService : IOperation<ListIdentity>
    {
        IRemoteCollection List(SearchListModel searchListModel);
    }

    public interface IListNonGenericEntityService : IOperation<ListNonGenericIdentity>
    {
        ListResult List(EntityName entityName, SearchListModel searchListModel);
    }
}