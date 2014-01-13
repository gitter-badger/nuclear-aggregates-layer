using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public sealed class ListOperationTypeService : ListEntityDtoServiceBase<OperationType, ListOperationTypeDto>
    {
        public ListOperationTypeService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListOperationTypeDto> GetListData(IQueryable<OperationType> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var syncCodeFilter = filterManager.CreateForExtendedProperty<OperationType, string>("excludeSyncCode", code => x => x.SyncCode1C != code);


            return query.ApplyFilter(syncCodeFilter)
                        .ApplyQuerySettings(querySettings, out count)
                        .Select(x => new
                            {
                                x.Id,
                                x.Name,
                                x.IsPlus
                            })
                        .AsEnumerable()
                        .Select(x => new ListOperationTypeDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                OperationTypeName = x.IsPlus ? BLResources.Charge : BLResources.Withdrawal
                            });
        }
    }
}