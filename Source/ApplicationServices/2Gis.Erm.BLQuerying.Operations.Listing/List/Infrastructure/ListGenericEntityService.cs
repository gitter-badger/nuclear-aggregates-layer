using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class ListGenericEntityService<TEntity> : IListGenericEntityService<TEntity>
        where TEntity : class, IEntityKey
    {
        private readonly IQuerySettingsProvider _querySettingsProvider;
        private readonly IFinderBaseProvider _finderBaseProvider;

        public ListGenericEntityService(IQuerySettingsProvider querySettingsProvider, IFinderBaseProvider finderBaseProvider)
        {
            _querySettingsProvider = querySettingsProvider;
            _finderBaseProvider = finderBaseProvider;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            int count;
            var entityType = typeof(TEntity);
            var entityName = entityType.AsEntityName();

            var finderBase = _finderBaseProvider.GetFinderBase(entityName);
            var query = finderBase.FindAll(entityType);
            var querySettings = _querySettingsProvider.GetQuerySettings(entityName, searchListModel);

            var dynamicList = query.ApplyQuerySettings(querySettings, out count).ToDynamicList(querySettings.Fields);

            return new DynamicListResult
                {
                    Data = dynamicList,
                    RowCount = count,
                    MainAttribute = querySettings.MainAttribute
                };
        }
    }
}