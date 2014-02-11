using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
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

        protected override IEnumerable<ListOperationTypeDto> GetListData(IQueryable<OperationType> query, QuerySettings querySettings, out int count)
        {
            var syncCodeFilter = querySettings.CreateForExtendedProperty<OperationType, string>("excludeSyncCode", code => x => x.SyncCode1C != code);


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