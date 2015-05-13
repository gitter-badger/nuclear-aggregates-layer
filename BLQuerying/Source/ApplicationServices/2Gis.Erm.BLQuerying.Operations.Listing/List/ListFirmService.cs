using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;
using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmService : ListEntityDtoServiceBase<Firm, ListFirmDto>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IQuery _query;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListFirmService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IQuery query,
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext)
        {
            _userIdentifierService = userIdentifierService;
            _query = query;
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Firm>();         

            bool excludeReserve;
            Expression<Func<Firm, bool>> excludeReserveFilter = null;
            if (querySettings.TryGetExtendedProperty("ExcludeReserve", out excludeReserve))
            {
                var reserveCode = _userIdentifierService.GetReserveUserIdentity().Code;
                excludeReserveFilter = x => x.OwnerCode != reserveCode;
            }

            long appendToDealId;
            if (querySettings.TryGetExtendedProperty("appendToDealId", out appendToDealId))
            {
                var clientId = _finder.Find(Specs.Find.ById<Deal>(appendToDealId)).Select(x => x.ClientId).Single();
                query = _filterHelper.ForClientAndItsDescendants(query, clientId);
            }

            bool forClientAndLinkedChild;
            if (querySettings.TryGetExtendedProperty("ForClientAndLinkedChild", out forClientAndLinkedChild) && querySettings.ParentEntityId.HasValue)
            {
                query = _filterHelper.ForClientAndLinkedChild(query, querySettings.ParentEntityId.Value);
            }

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var myTerritoryFilter = querySettings.CreateForExtendedProperty<Firm, bool>(
                "MyTerritory",
                info =>
                    {
                        var userId = _userContext.Identity.Code;
                        return x => x.Territory.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
                    });

            var myBranchFilter = querySettings.CreateForExtendedProperty<Firm, bool>(
                "MyBranch",
                info =>
                    {
                        var userId = _userContext.Identity.Code;
                        return x => x.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
                    });

            var selfAdsOrdersFilter = querySettings.CreateForExtendedProperty<Firm, bool>(
                "WithSelfAdsOrders",
                info => x => x.Orders.Any(y => !y.IsDeleted && y.IsActive && y.OrderType == OrderType.SelfAds));

            var dealFilter = querySettings.CreateForExtendedProperty<Firm, long>(
                "dealId",
                dealId =>
                    {
                        var dealFirms = _finder.Find(DealSpecs.FirmDeals.Find.ByDeal(dealId) && Specs.Find.NotDeleted<FirmDeal>()).Select(x => x.FirmId).ToArray();

                        if (dealFirms.Any())
                        {
                            return x => dealFirms.Contains(x.Id);
                        }

                        var clientId = _finder.Find(Specs.Find.ById<Deal>(dealId)).Select(x => x.ClientId).Single();
                        return x => x.ClientId == clientId;
                    });

            // TODO {all, 20.10.2014}: этот clientFilter нужен только при создании заказа не из сделки. После того, как такая возможность исчезнет необходимо убрать этот код и передачу clientId c клиента.
            Expression<Func<Firm, bool>> clientFilter = null;
            long orderDealId;
            if (!querySettings.TryGetExtendedProperty("dealId", out orderDealId))
            {
                clientFilter = querySettings.CreateForExtendedProperty<Firm, long>("clientId", clientId => x => x.ClientId == clientId);
            }

            return
                query.Where(x => !x.IsDeleted)
                     .Filter(_filterHelper, dealFilter, excludeReserveFilter, clientFilter, myTerritoryFilter, myBranchFilter, selfAdsOrdersFilter)
                     .Select(
                         x =>
                         new ListFirmDto
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 CreatedOn = x.CreatedOn,
                                 ClientId = x.Client != null ? x.Client.Id : (long?)null,
                                 ClientName = x.Client != null ? x.Client.Name : null,
                                 OwnerCode = x.OwnerCode,
                                 TerritoryId = x.Territory != null ? x.Territory.Id : (long?)null,
                                 TerritoryName = x.Territory != null ? x.Territory.Name : null,
                                 PromisingScore = x.PromisingScore,
                                 LastQualifyTime = x.LastQualifyTime,
                                 LastDisqualifyTime = x.LastDisqualifyTime,
                                 OrganizationUnitId = x.OrganizationUnit.Id,
                                 OrganizationUnitName = x.OrganizationUnit.Name,
                                 IsActive = x.IsActive,
                                 IsDeleted = x.IsDeleted,
                                 ClosedForAscertainment = x.ClosedForAscertainment,
                                 OwnerName = null,
                        IsOwner = x.OwnerCode == _userContext.Identity.Code
                             })
                     .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListFirmDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}