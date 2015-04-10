using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.API.Operations.Docs;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class RelationalMetadata
    {
        private static readonly Dictionary<Tuple<Type, EntityName>, LambdaExpression> RelationalMap = new Dictionary<Tuple<Type, EntityName>, LambdaExpression>()

            .RegisterRelatedFilter<ListAccountDetailDto>(EntityName.Account, x => x.AccountId)

            .RegisterRelatedFilter<ListAccountDto>(EntityName.LegalPerson, x => x.LegalPersonId)

            .RegisterRelatedFilter<ListLockDto>(EntityName.Account, x => x.AccountId)
            .RegisterRelatedFilter<ListLockDto>(EntityName.Order, x => x.OrderId)

            .RegisterRelatedFilter<ListLimitDto>(EntityName.Account, x => x.AccountId)
            .RegisterRelatedFilter<ListLimitDto>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<ListLimitDto>(EntityName.LegalPerson, x => x.LegalPersonId)

            .RegisterRelatedFilter<ListOrderDto>(EntityName.Account, x => x.AccountId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Deal, x => x.DealId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Firm, x => x.FirmId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.LegalPerson, x => x.LegalPersonId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Bargain, x => x.BargainId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityName.Account, x => x.AccountId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityName.Deal, x => x.DealId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityName.Firm, x => x.FirmId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityName.LegalPerson, x => x.LegalPersonId)
            .RegisterRelatedFilter<OrderGridDoc>(EntityName.Bargain, x => x.BargainId)

            .RegisterRelatedFilter<ListAdsTemplatesAdsElementTemplateDto>(EntityName.AdvertisementTemplate, x => x.AdsTemplateId)
            .RegisterRelatedFilter<ListAdsTemplatesAdsElementTemplateDto>(EntityName.AdvertisementElementTemplate, x => x.AdsElementTemplateId)

            .RegisterRelatedFilter<ListAdvertisementElementDenialReasonsDto>(EntityName.AdvertisementElement, x => x.AdvertisementElementId)

            .RegisterRelatedFilter<ListAssociatedPositionDto>(EntityName.AssociatedPositionsGroup, x => x.AssociatedPositionsGroupId)

            .RegisterRelatedFilter<ListBargainFileDto>(EntityName.Bargain, x => x.BargainId)

            .RegisterRelatedFilter<ListBranchOfficeOrganizationUnitDto>(EntityName.BranchOffice, x => x.BranchOfficeId)
            .RegisterRelatedFilter<ListBranchOfficeOrganizationUnitDto>(EntityName.OrganizationUnit, x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListPrintFormTemplateDto>(EntityName.BranchOfficeOrganizationUnit, x => x.BranchOfficeOrganizationUnitId)

            .RegisterRelatedFilter<ListCategoryDto>(EntityName.Category, x => x.ParentId)

            .RegisterRelatedFilter<ListCategoryOrganizationUnitDto>(EntityName.Category, x => x.CategoryId)

            .RegisterRelatedFilter<ListFirmDto>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<ListFirmDto>(EntityName.Territory, x => x.TerritoryId)
            .RegisterRelatedFilter<FirmGridDoc>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<FirmGridDoc>(EntityName.Territory, x => x.TerritoryId)
            .RegisterRelatedFilter<ListFirmDealDto>(EntityName.Deal, x => x.DealId)

            .RegisterRelatedFilter<ListLegalPersonDealDto>(EntityName.Deal, x => x.DealId)

            .RegisterRelatedFilter<ListContactDto>(EntityName.Client, x => x.ClientId)

            .RegisterRelatedFilter<ListDealDto>(EntityName.Client, x => x.ClientId)

            .RegisterRelatedFilter<ListLegalPersonDto>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<LegalPersonGridDoc>(EntityName.Client, x => x.ClientId)

            .RegisterRelatedFilter<ListBargainDto>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<ListBargainDto>(EntityName.LegalPerson, x => x.LegalPersonId)
            .RegisterRelatedFilter<BargainGridDoc>(EntityName.Client, x => x.ClientId)
            .RegisterRelatedFilter<BargainGridDoc>(EntityName.LegalPerson, x => x.LegalPersonId)

            .RegisterRelatedFilter<ListCurrencyRateDto>(EntityName.Currency, x => x.CurrencyId)

            .RegisterRelatedFilter<ListCountryDto>(EntityName.Currency, x => x.CurrencyId)
            .RegisterRelatedFilter<CountryGridDoc>(EntityName.Currency, x => x.CurrencyId)

            .RegisterRelatedFilter<ListFirmAddressDto>(EntityName.Firm, x => x.FirmId)

            .RegisterRelatedFilter<ListCategoryFirmAddressDto>(EntityName.Firm, x => x.FirmId)
            .RegisterRelatedFilter<ListCategoryFirmAddressDto>(EntityName.FirmAddress, x => x.FirmAddressId)

            .RegisterRelatedFilter<ListAdvertisementDto>(EntityName.Firm, x => x.FirmId)

            .RegisterRelatedFilter<ListFirmContactDto>(EntityName.FirmAddress, x => x.FirmAddressId)

            .RegisterRelatedFilter<ListLockDetailDto>(EntityName.Lock, x => x.LockId)

            .RegisterRelatedFilter<ListBillDto>(EntityName.Order, x => x.OrderId)

            .RegisterRelatedFilter<ListOrderFileDto>(EntityName.Order, x => x.OrderId)

            .RegisterRelatedFilter<ListOrderProcessingRequestDto>(EntityName.Order, x => x.RenewedOrderId)

            .RegisterRelatedFilter<ListPriceDto>(EntityName.OrganizationUnit, x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListProjectDto>(EntityName.OrganizationUnit, x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListPositionChildrenDto>(EntityName.Position, x => x.MasterPositionId)

            .RegisterRelatedFilter<ListAssociatedPositionsGroupDto>(EntityName.PricePosition, x => x.PricePositionId)

            .RegisterRelatedFilter<ListPricePositionDto>(EntityName.Price, x => x.PriceId)

            .RegisterRelatedFilter<ListThemeOrganizationUnitDto>(EntityName.Theme, x => x.ThemeId)

            .RegisterRelatedFilter<ListThemeCategoryDto>(EntityName.Theme, x => x.ThemeId)

            .RegisterRelatedFilter<ListUserRoleDto>(EntityName.User, x => x.UserId)

            .RegisterRelatedFilter<ListUserTerritoryDto>(EntityName.User, x => x.UserId)

            .RegisterRelatedFilter<ListUserBranchOfficeDto>(EntityName.BranchOffice, x => x.BranchOfficeId)

            .RegisterRelatedFilter<ListUserOrganizationUnitDto>(EntityName.User, x => x.UserId)
            .RegisterRelatedFilter<ListUserOrganizationUnitDto>(EntityName.OrganizationUnit, x => x.OrganizationUnitId)

            .RegisterRelatedFilter<ListAdvertisementElementDto>(EntityName.Advertisement, x => x.AdvertisementId)

            .RegisterRelatedFilter<ListLegalPersonProfileDto>(EntityName.LegalPerson, x => x.LegalPersonId)

            .RegisterRelatedFilter<ListOrderPositionDto>(EntityName.Order, x => x.OrderId)
            ;

        private static Dictionary<Tuple<Type, EntityName>, LambdaExpression> RegisterRelatedFilter<TDocument>(this Dictionary<Tuple<Type, EntityName>, LambdaExpression> map, EntityName parentEntityName, Expression<Func<TDocument, object>> expression)
        {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);
            map.Add(key, expression);
            return map;
        }

        // может вызываться несколько раз, поэтому есть ContainsKey
        public static void RegisterRelatedFilter<TDocument>(EntityName parentEntityName, Expression<Func<TDocument, object>> expression)
        {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);

            if (!RelationalMap.ContainsKey(key))
            {
                RelationalMap.RegisterRelatedFilter(parentEntityName, expression);
            }
        }

        public static bool TryGetFilterExpressionFromRelationalMap<TDocument>(EntityName parentEntityName, out LambdaExpression lambdaExpression)
            {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);
            return RelationalMap.TryGetValue(key, out lambdaExpression);
        }
    }
}