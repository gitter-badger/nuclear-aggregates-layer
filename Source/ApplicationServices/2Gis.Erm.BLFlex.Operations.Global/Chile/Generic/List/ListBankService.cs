using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.List
{
    public sealed class ListBankService : IListGenericEntityService<Bank>
    {
        private readonly IFinder _finder;
        private readonly IQuerySettingsProvider _querySettingsProvider;

        public ListBankService(IFinder finder, IQuerySettingsProvider querySettingsProvider)
        {
            _finder = finder;
            _querySettingsProvider = querySettingsProvider;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            int count;
            var entityType = typeof(Bank);
            var entityName = entityType.AsEntityName();

            var querySettings = _querySettingsProvider.GetQuerySettings(entityName, searchListModel);

            var dynamicList = _finder.FindAll<DictionaryEntityInstance>()
                                     .Where(BankSpecifications.FindOnlyBanks())
                                     .Select(BankSpecifications.Select().Selector)
                                     .ApplyQuerySettings(querySettings, out count)
                                     .ToDynamicList(querySettings.Fields);

            return new DynamicListResult
                {
                    Data = dynamicList, 
                    RowCount = count, 
                    MainAttribute = querySettings.MainAttribute
                };
        }
    }
}