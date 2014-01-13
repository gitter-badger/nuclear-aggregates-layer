using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListDealService : ListEntityDtoServiceBase<Deal, ListDealDto>
    {
        public ListDealService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListDealDto> GetListData(IQueryable<Deal> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var filterExpression = filterManager.CreateForExtendedProperty<Deal, long>(
                "OrderId", 
                orderId =>
                {
                    var orderInfo = FinderBaseProvider
                            .GetFinderBase(typeof(Order).AsEntityName())
                            .FindAll<Order>()
                            .Where(x => x.Id == orderId)
                            .Select(x => new
                                {
                                    x.DealId,
                                    DealClientId = (long?)x.Deal.ClientId,
                                    LegalPersonClientId = x.LegalPerson.ClientId,
                                    DestOrganizationUnitId = (long?)x.DestOrganizationUnitId
                                })
                            .Single();
                        if (orderInfo.DealId.HasValue)
                        {
                            return x => x.Id != orderInfo.DealId.Value && x.ClientId == orderInfo.DealClientId.Value;
                        }

                        if (orderInfo.DestOrganizationUnitId.HasValue)
                        {
                            if (orderInfo.LegalPersonClientId.HasValue)
                            {
                                return x => x.ClientId == orderInfo.LegalPersonClientId.Value;
                            }

                            return x => x.Client.Territory.OrganizationUnitId == orderInfo.DestOrganizationUnitId.Value &&
                                        !x.Client.IsDeleted &&
                                        x.Client.Territory.IsActive;
                        }

                        return null;
                    });

            return query
                .ApplyFilter(filterExpression)
                .ApplyQuerySettings(querySettings, out count)
                .Select(d =>
                        new ListDealDto
                            {
                                Id = d.Id,
                                Name = d.Name,
                                ClientId = d.ClientId,
                                ClientName = d.Client.Name,
                                MainFirmId = d.MainFirmId,
                                MainFirmName = d.Firm.Name
                            });
        }
    }
}