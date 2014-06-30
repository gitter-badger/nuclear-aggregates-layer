using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public abstract class ListEntityDtoServiceBase<TEntity, TEntityListDto> : IListGenericEntityDtoService<TEntity, TEntityListDto>
        where TEntity : class,  IEntity, IEntityKey
        where TEntityListDto : IOperationSpecificEntityDto
    {
        public IRemoteCollection List(SearchListModel searchListModel)
        {
            var querySettings = searchListModel.ToQuerySettings();
            var remoteCollection = List(querySettings);
            return remoteCollection;
        }

        protected abstract IRemoteCollection List(QuerySettings querySettings);
    }
}