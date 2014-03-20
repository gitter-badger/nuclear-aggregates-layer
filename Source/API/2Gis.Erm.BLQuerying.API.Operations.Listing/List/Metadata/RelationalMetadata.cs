using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class RelationalMetadata
    {
        private static readonly Dictionary<Tuple<Type, EntityName>, Func<long?, Expression>> RelationalMap = new Dictionary<Tuple<Type, EntityName>, Func<long?, Expression>>()

            .RegisterRelatedFilter<ListAccountDetailDto>(EntityName.Account, parentId => x => x.AccountId == parentId)

            .RegisterRelatedFilter<ListAccountDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId)

            .RegisterRelatedFilter<ListActivityInstanceDto>(EntityName.Client, parentId => x => x.ClientId == parentId)
            .RegisterRelatedFilter<ListActivityInstanceDto>(EntityName.Contact, parentId => x => x.ContactId == parentId)
            .RegisterRelatedFilter<ListActivityInstanceDto>(EntityName.Deal, parentId => x => x.DealId == parentId)
            .RegisterRelatedFilter<ListActivityInstanceDto>(EntityName.Firm, parentId => x => x.FirmId == parentId)

            .RegisterRelatedFilter<ListLockDto>(EntityName.Account, parentId => x => x.AccountId == parentId)
            .RegisterRelatedFilter<ListLockDto>(EntityName.Order, parentId => x => x.OrderId == parentId)

            .RegisterRelatedFilter<ListLimitDto>(EntityName.Account, parentId => x => x.AccountId == parentId)
            .RegisterRelatedFilter<ListLimitDto>(EntityName.Client, parentId => x => x.ClientId == parentId)
            .RegisterRelatedFilter<ListLimitDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId)

            .RegisterRelatedFilter<ListOrderDto>(EntityName.Account, parentId => x => x.AccountId == parentId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Client, parentId => x => x.ClientId == parentId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Deal, parentId => x => x.DealId == parentId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.Firm, parentId => x => x.FirmId == parentId)
            .RegisterRelatedFilter<ListOrderDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId)

            .RegisterRelatedFilter<ListAdsTemplatesAdsElementTemplateDto>(EntityName.AdvertisementTemplate, parentId => x => x.AdsTemplateId == parentId)
            .RegisterRelatedFilter<ListAdsTemplatesAdsElementTemplateDto>(EntityName.AdvertisementElementTemplate, parentId => x => x.AdsElementTemplateId == parentId)

            .RegisterRelatedFilter<ListAssociatedPositionDto>(EntityName.AssociatedPositionsGroup, parentId => x => x.AssociatedPositionsGroupId == parentId)

            .RegisterRelatedFilter<ListBargainFileDto>(EntityName.Bargain, parentId => x => x.BargainId == parentId)

            .RegisterRelatedFilter<ListBranchOfficeOrganizationUnitDto>(EntityName.BranchOffice, parentId => x => x.BranchOfficeId == parentId)
            .RegisterRelatedFilter<ListBranchOfficeOrganizationUnitDto>(EntityName.OrganizationUnit, parentId => x => x.OrganizationUnitId == parentId)

            .RegisterRelatedFilter<ListPrintFormTemplateDto>(EntityName.BranchOfficeOrganizationUnit, parentId => x => x.BranchOfficeOrganizationUnitId == parentId)

            .RegisterRelatedFilter<ListCategoryDto>(EntityName.Category, parentId => x => x.ParentId == parentId)

            .RegisterRelatedFilter<ListCategoryOrganizationUnitDto>(EntityName.Category, parentId => x => x.CategoryId == parentId)

            .RegisterRelatedFilter<ListFirmDto>(EntityName.Client, parentId => x => x.ClientId == parentId)
            .RegisterRelatedFilter<ListFirmDto>(EntityName.Territory, parentId => x => x.TerritoryId == parentId)

            .RegisterRelatedFilter<ListContactDto>(EntityName.Client, parentId => x => x.ClientId == parentId)

            .RegisterRelatedFilter<ListDealDto>(EntityName.Client, parentId => x => x.ClientId == parentId)

            .RegisterRelatedFilter<ListLegalPersonDto>(EntityName.Client, parentId => x => x.ClientId == parentId)

            .RegisterRelatedFilter<ListBargainDto>(EntityName.Client, parentId => x => x.ClientId == parentId)
            .RegisterRelatedFilter<ListBargainDto>(EntityName.LegalPerson, parentId => x => x.CustomerLegalPersonId == parentId)

            .RegisterRelatedFilter<ListCurrencyRateDto>(EntityName.Currency, parentId => x => x.CurrencyId == parentId)

            .RegisterRelatedFilter<ListCountryDto>(EntityName.Currency, parentId => x => x.CurrencyId == parentId)

            .RegisterRelatedFilter<ListFirmAddressDto>(EntityName.Firm, parentId => x => x.FirmId == parentId)

            .RegisterRelatedFilter<ListCategoryFirmAddressDto>(EntityName.Firm, parentId => x => x.FirmId == parentId)
            .RegisterRelatedFilter<ListCategoryFirmAddressDto>(EntityName.FirmAddress, parentId => x => x.FirmAddressId == parentId)

            .RegisterRelatedFilter<ListAdvertisementDto>(EntityName.Firm, parentId => x => x.FirmId == parentId)

            .RegisterRelatedFilter<ListFirmContactDto>(EntityName.FirmAddress, parentId => x => x.FirmAddressId == parentId || (x.FirmAddressId == null && x.CardId != null))

            .RegisterRelatedFilter<ListLockDetailDto>(EntityName.Lock, parentId => x => x.LockId == parentId)

            .RegisterRelatedFilter<ListBillDto>(EntityName.Order, parentId => x => x.OrderId == parentId)

            .RegisterRelatedFilter<ListOrderFileDto>(EntityName.Order, parentId => x => x.OrderId == parentId)

            .RegisterRelatedFilter<ListOrderProcessingRequestDto>(EntityName.Order, parentId => x => x.RenewedOrderId == parentId)

            .RegisterRelatedFilter<ListPriceDto>(EntityName.OrganizationUnit, parentId => x => x.OrganizationUnitId == parentId)

            .RegisterRelatedFilter<ListProjectDto>(EntityName.OrganizationUnit, parentId => x => x.OrganizationUnitId == parentId)

            .RegisterRelatedFilter<ListPositionChildrenDto>(EntityName.Position, parentId => x => x.MasterPositionId == parentId)

            .RegisterRelatedFilter<ListAssociatedPositionsGroupDto>(EntityName.PricePosition, parentId => x => x.PricePositionId == parentId)

            .RegisterRelatedFilter<ListPricePositionDto>(EntityName.Price, parentId => x => x.PriceId == parentId)

            .RegisterRelatedFilter<ListThemeOrganizationUnitDto>(EntityName.Theme, parentId => x => x.ThemeId == parentId)

            .RegisterRelatedFilter<ListThemeCategoryDto>(EntityName.Theme, parentId => x => x.ThemeId == parentId)

            .RegisterRelatedFilter<ListUserRoleDto>(EntityName.User, parentId => x => x.UserId == parentId)

            .RegisterRelatedFilter<ListUserTerritoryDto>(EntityName.User, parentId => x => x.UserId == parentId)

            .RegisterRelatedFilter<ListUserOrganizationUnitDto>(EntityName.User, parentId => x => x.UserId == parentId)
            .RegisterRelatedFilter<ListUserOrganizationUnitDto>(EntityName.OrganizationUnit, parentId => x => x.OrganizationUnitId == parentId)

            .RegisterRelatedFilter<ListAdvertisementElementDto>(EntityName.Advertisement, parentId => x => x.AdvertisementId == parentId)

            .RegisterRelatedFilter<ListLegalPersonProfileDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId)

            .RegisterRelatedFilter<ListOrderPositionDto>(EntityName.Order, parentId => x => x.OrderId == parentId)
            ;

        private static Dictionary<Tuple<Type, EntityName>, Func<long?, Expression>> RegisterRelatedFilter<TDocument>(this Dictionary<Tuple<Type, EntityName>, Func<long?, Expression>> map, EntityName parentEntityName, Func<long?, Expression<Func<TDocument, bool>>> func)
        {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);
            map.Add(key, func);
            return map;
        }

        // может вызываться несколько раз, поэтому есть ContainsKey
        public static void RegisterRelatedFilter<TDocument>(EntityName parentEntityName, Func<long?, Expression<Func<TDocument, bool>>> func)
        {
            var key = Tuple.Create(typeof(TDocument), parentEntityName);

            if (!RelationalMap.ContainsKey(key))
            {
                RelationalMap.RegisterRelatedFilter(parentEntityName, func);
            }
        }

        public static bool TryGetFilterExpressionFromRelationalMap<TDocument>(EntityName parentEntityName, long? parentEntityId, out Expression expression)
        {
            Func<long?, Expression> func;
            if (RelationalMap.TryGetValue(Tuple.Create(typeof(TDocument), parentEntityName), out func))
            {
                expression = func(parentEntityId);
                return true;
            }

            expression = null;
            return false;
        }
    }
}