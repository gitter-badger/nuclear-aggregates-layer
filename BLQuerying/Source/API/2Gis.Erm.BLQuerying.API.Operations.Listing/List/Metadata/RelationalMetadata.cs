using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.API.Operations.Docs;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class RelationalMetadata
    {
        private static readonly Dictionary<Tuple<Type, IEntityType>, LambdaExpression> RelationalMap = new Dictionary<Tuple<Type, IEntityType>, LambdaExpression>()

            .RegisterRelatedFilter<ListAccountDetailDto>(EntityType.Instance.Account(), x => x.AccountId)

            .RegisterRelatedFilter<ListAccountDto>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)

            .RegisterRelatedFilter<ListLockDto>(EntityType.Instance.Account(), x => x.AccountId)
            .RegisterRelatedFilter<ListLockDto>(EntityType.Instance.Order(), x => x.OrderId)

            .RegisterRelatedFilter<ListLimitDto>(EntityType.Instance.Account(), x => x.AccountId)
            .RegisterRelatedFilter<ListLimitDto>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<ListLimitDto>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)

            .RegisterRelatedFilter<ListOrderDto>(EntityType.Instance.Account(), x => x.AccountId)
            .RegisterRelatedFilter<ListOrderDto>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<ListOrderDto>(EntityType.Instance.Deal(), x => x.DealId)
            .RegisterRelatedFilter<ListOrderDto>(EntityType.Instance.Firm(), x => x.FirmId)
            .RegisterRelatedFilter<ListOrderDto>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)
            .RegisterRelatedFilter<ListOrderDto>(EntityType.Instance.Bargain(), x => x.BargainId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityType.Instance.Account(), x => x.AccountId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityType.Instance.Deal(), x => x.DealId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityType.Instance.Firm(), x => x.FirmId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityType.Instance.Bargain(), x => x.BargainId)

            .RegisterRelatedFilter<ListAdsTemplatesAdsElementTemplateDto>(EntityType.Instance.AdvertisementTemplate(), x => x.AdsTemplateId)
            .RegisterRelatedFilter<ListAdsTemplatesAdsElementTemplateDto>(EntityType.Instance.AdvertisementElementTemplate(), x => x.AdsElementTemplateId)

            .RegisterRelatedFilter<ListAdvertisementElementDenialReasonsDto>(EntityType.Instance.AdvertisementElement(), x => x.AdvertisementElementId)

            .RegisterRelatedFilter<ListAssociatedPositionDto>(EntityType.Instance.AssociatedPositionsGroup(), x => x.AssociatedPositionsGroupId)

            .RegisterRelatedFilter<ListBargainFileDto>(EntityType.Instance.Bargain(), x => x.BargainId)

            .RegisterRelatedFilter<ListBranchOfficeOrganizationUnitDto>(EntityType.Instance.BranchOffice(), x => x.BranchOfficeId)
            .RegisterRelatedFilter<ListBranchOfficeOrganizationUnitDto>(EntityType.Instance.OrganizationUnit(), x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListPrintFormTemplateDto>(EntityType.Instance.BranchOfficeOrganizationUnit(), x => x.BranchOfficeOrganizationUnitId)

            .RegisterRelatedFilter<ListCategoryDto>(EntityType.Instance.Category(), x => x.ParentId)

            .RegisterRelatedFilter<ListCategoryOrganizationUnitDto>(EntityType.Instance.Category(), x => x.CategoryId)

            .RegisterRelatedFilter<ListFirmDto>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<ListFirmDto>(EntityType.Instance.Territory(), x => x.TerritoryId)
            .RegisterRelatedFilter<FirmGridDoc>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<FirmGridDoc>(EntityType.Instance.Territory(), x => x.TerritoryId)
            .RegisterRelatedFilter<ListFirmDealDto>(EntityType.Instance.Deal(), x => x.DealId)

            .RegisterRelatedFilter<ListLegalPersonDealDto>(EntityType.Instance.Deal(), x => x.DealId)

            .RegisterRelatedFilter<ListContactDto>(EntityType.Instance.Client(), x => x.ClientId)

            .RegisterRelatedFilter<ListDealDto>(EntityType.Instance.Client(), x => x.ClientId)

            .RegisterRelatedFilter<ListLegalPersonDto>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<LegalPersonGridDoc>(EntityType.Instance.Client(), x => x.ClientId)

            .RegisterRelatedFilter<ListBargainDto>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<ListBargainDto>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)
            .RegisterRelatedFilter<BargainGridDoc>(EntityType.Instance.Client(), x => x.ClientId)
            .RegisterRelatedFilter<BargainGridDoc>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)

            .RegisterRelatedFilter<ListCurrencyRateDto>(EntityType.Instance.Currency(), x => x.CurrencyId)

            .RegisterRelatedFilter<ListCountryDto>(EntityType.Instance.Currency(), x => x.CurrencyId)
            .RegisterRelatedFilter<CountryGridDoc>(EntityType.Instance.Currency(), x => x.CurrencyId)

            .RegisterRelatedFilter<ListFirmAddressDto>(EntityType.Instance.Firm(), x => x.FirmId)

            .RegisterRelatedFilter<ListCategoryFirmAddressDto>(EntityType.Instance.Firm(), x => x.FirmId)
            .RegisterRelatedFilter<ListCategoryFirmAddressDto>(EntityType.Instance.FirmAddress(), x => x.FirmAddressId)

            .RegisterRelatedFilter<ListAdvertisementDto>(EntityType.Instance.Firm(), x => x.FirmId)

            .RegisterRelatedFilter<ListFirmContactDto>(EntityType.Instance.FirmAddress(), x => x.FirmAddressId)

            .RegisterRelatedFilter<ListLockDetailDto>(EntityType.Instance.Lock(), x => x.LockId)

            .RegisterRelatedFilter<ListBillDto>(EntityType.Instance.Order(), x => x.OrderId)

            .RegisterRelatedFilter<ListOrderFileDto>(EntityType.Instance.Order(), x => x.OrderId)

            .RegisterRelatedFilter<ListOrderProcessingRequestDto>(EntityType.Instance.Order(), x => x.RenewedOrderId)

            .RegisterRelatedFilter<ListPriceDto>(EntityType.Instance.OrganizationUnit(), x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListProjectDto>(EntityType.Instance.OrganizationUnit(), x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListPositionChildrenDto>(EntityType.Instance.Position(), x => x.MasterPositionId)

            .RegisterRelatedFilter<ListAssociatedPositionsGroupDto>(EntityType.Instance.PricePosition(), x => x.PricePositionId)

            .RegisterRelatedFilter<ListPricePositionDto>(EntityType.Instance.Price(), x => x.PriceId)

            .RegisterRelatedFilter<ListThemeOrganizationUnitDto>(EntityType.Instance.Theme(), x => x.ThemeId)

            .RegisterRelatedFilter<ListThemeCategoryDto>(EntityType.Instance.Theme(), x => x.ThemeId)

            .RegisterRelatedFilter<ListUserRoleDto>(EntityType.Instance.User(), x => x.UserId)

            .RegisterRelatedFilter<ListUserTerritoryDto>(EntityType.Instance.User(), x => x.UserId)

            .RegisterRelatedFilter<ListUserOrganizationUnitDto>(EntityType.Instance.User(), x => x.UserId)
            .RegisterRelatedFilter<ListUserOrganizationUnitDto>(EntityType.Instance.OrganizationUnit(), x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListAdvertisementElementDto>(EntityType.Instance.Advertisement(), x => x.AdvertisementId)

            .RegisterRelatedFilter<ListLegalPersonProfileDto>(EntityType.Instance.LegalPerson(), x => x.LegalPersonId)

            .RegisterRelatedFilter<ListOrderPositionDto>(EntityType.Instance.Order(), x => x.OrderId);

        private static Dictionary<Tuple<Type, IEntityType>, LambdaExpression> RegisterRelatedFilter<TDocument>(
            this Dictionary<Tuple<Type, IEntityType>, LambdaExpression> map,
            IEntityType parentEntityName,
            Expression<Func<TDocument, object>> expression)
        {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);
            map.Add(key, expression);
            return map;
        }

        // может вызываться несколько раз, поэтому есть ContainsKey
        public static void RegisterRelatedFilter<TDocument>(IEntityType parentEntityName, Expression<Func<TDocument, object>> expression)
        {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);

            if (!RelationalMap.ContainsKey(key))
            {
                RelationalMap.RegisterRelatedFilter(parentEntityName, expression);
            }
        }

        public static bool TryGetFilterExpressionFromRelationalMap<TDocument>(IEntityType parentEntityName, out LambdaExpression lambdaExpression)
            {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);
            return RelationalMap.TryGetValue(key, out lambdaExpression);
        }
    }
}