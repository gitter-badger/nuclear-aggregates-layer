using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IListEntityService : IOperation<ListIdentity>
    {
        IRemoteCollection List(SearchListModel searchListModel);
    }

    public interface IListNonGenericEntityService : IOperation<ListNonGenericIdentity>
    {
        ListResult List(IEntityType entityName, SearchListModel searchListModel);
    }
}