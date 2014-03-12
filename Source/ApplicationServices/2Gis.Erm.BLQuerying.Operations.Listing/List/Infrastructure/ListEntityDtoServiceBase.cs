using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public abstract class ListEntityDtoServiceBase<TEntity, TEntityListDto> : IListGenericEntityDtoService<TEntity, TEntityListDto>
        where TEntity : class,  IEntity, IEntityKey
        where TEntityListDto : IListItemEntityDto<TEntity>
    {
        private static readonly EntityName EntityName = typeof(TEntity).AsEntityName();
        private readonly IQuerySettingsProvider _querySettingsProvider;

        protected ListEntityDtoServiceBase(IQuerySettingsProvider querySettingsProvider)
        {
            _querySettingsProvider = querySettingsProvider;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            var querySettings = _querySettingsProvider.GetQuerySettings(EntityName, typeof(TEntityListDto), searchListModel);

            int count;
            var data = List(querySettings, out count).ToArray();

            return new EntityDtoListResult<TEntity, TEntityListDto>
            {
                Data = data,
                RowCount = count,
            };
        }

        protected abstract IEnumerable<TEntityListDto> List(QuerySettings querySettings, out int count);
    }
}