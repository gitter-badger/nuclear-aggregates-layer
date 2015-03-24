using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Qds.API.Operations.Docs;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class FilteredFieldsMetadata
    {
        private static readonly Dictionary<Type, LambdaExpression[]> FilteredFieldsMap = new Dictionary<Type, LambdaExpression[]>()
            .RegisterFilteredFields<ListAccountDto>(
                x => x.BranchOfficeOrganizationUnitName,
                x => x.LegalPersonName,
                x => x.ClientName,
                x => x.Inn)
            .RegisterFilteredFields<ListAccountDetailDto>(
                x => x.OperationType,
                x => x.Description)
            .RegisterFilteredFields<ListActivityDto>(
                x => x.Header)
            .RegisterFilteredFields<ListAdditionalFirmServiceDto>(
                x => x.ServiceCode,
                x => x.Description)
            .RegisterFilteredFields<ListAdsTemplatesAdsElementTemplateDto>(
                x => x.AdsTemplateName,
                x => x.AdsElementTemplateName)
            .RegisterFilteredFields<ListDenialReasonDto>(
                x => x.Name)
            .RegisterFilteredFields<ListAdvertisementTemplateDto>(
                x => x.Name)
            .RegisterFilteredFields<ListAdvertisementElementTemplateDto>(
                x => x.Name)
            .RegisterFilteredFields<ListAdvertisementDto>(
                x => x.Name,
                x => x.AdvertisementTemplateName)
            .RegisterFilteredFields<ListAdvertisementElementDto>(
                x => x.AdvertisementElementTemplateName)
            .RegisterFilteredFields<ListAdvertisementElementDenialReasonsDto>(
                x => x.DenialReasonName)
            .RegisterFilteredFields<ListAssociatedPositionDto>(
                x => x.PositionName)
            .RegisterFilteredFields<ListAssociatedPositionsGroupDto>(
                x => x.Name)
            .RegisterFilteredFields<ListBargainDto>(
                x => x.Number,
                x => x.LegalPersonLegalName,
                x => x.LegalPersonLegalAddress,
                x => x.ClientName,
                x => x.BranchOfficeName)
            .RegisterFilteredFields<BargainGridDoc>(
                x => x.Number,
                x => x.LegalPersonLegalName,
                x => x.LegalPersonLegalAddress,
                x => x.OwnerName,
                x => x.BargainKind)
            .RegisterFilteredFields<ListBargainFileDto>(
                x => x.FileName)
            .RegisterFilteredFields<ListBargainTypeDto>(
                x => x.Name)
            .RegisterFilteredFields<ListBillDto>(
                x => x.Number,
                x => x.ClientName,
                x => x.FirmName)
            .RegisterFilteredFields<ListBranchOfficeDto>(
                x => x.Name,
                x => x.Inn,
                x => x.Ic,
                x => x.Rut,
                x => x.LegalAddress)
            .RegisterFilteredFields<ListBranchOfficeOrganizationUnitDto>(
                x => x.ShortLegalName,
                x => x.BranchOfficeName,
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListCategoryDto>(
                x => x.Name)
            .RegisterFilteredFields<ListCategoryGroupDto>(
                x => x.Name)
            .RegisterFilteredFields<ListCategoryFirmAddressDto>(
                x => x.Name)
            .RegisterFilteredFields<ListCategoryOrganizationUnitDto>(
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListClientDto>(
                x => x.Name)
            .RegisterFilteredFields<ListClientLinkDto>(
                x => x.ChildClientName,
                x => x.MasterClientName)
            .RegisterFilteredFields<ClientGridDoc>(
                x => x.Name,
                x => x.MainAddress,
                x => x.MainFirmName,
                x => x.TerritoryName,
                x => x.MainPhoneNumber,
                x => x.OwnerName)
            .RegisterFilteredFields<ListContactDto>(
                x => x.FullName,
                x => x.WorkAddress,
                x => x.Website,
                x => x.WorkEmail,
                x => x.HomeEmail)
            .RegisterFilteredFields<ListContributionTypeDto>(
                x => x.Name)
            .RegisterFilteredFields<ListCountryDto>(
                x => x.IsoCode,
                x => x.CurrencyName,
                x => x.Name)
            .RegisterFilteredFields<ListCurrencyDto>(
                x => x.ISOCode,
                x => x.Name)
            .RegisterFilteredFields<ListCurrencyRateDto>(
                x => x.CurrencyName)
            .RegisterFilteredFields<ListDealDto>(
                x => x.Name,
                x => x.ClientName,
                x => x.MainFirmName)
            .RegisterFilteredFields<ListDeniedPositionDto>(
                x => x.PositionDeniedName)
            .RegisterFilteredFields<ListDepartmentDto>(
                x => x.Name,
                x => x.ParentName)
            .RegisterFilteredFields<DepartmentGridDoc>(
                x => x.Name,
                x => x.ParentName)
            .RegisterFilteredFields<ListFirmDto>(
                x => x.Name,
                x => x.ClientName,
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<FirmGridDoc>(
                x => x.Name,
                x => x.ClientName,
                x => x.OrganizationUnitName,
                x => x.OwnerName)
            .RegisterFilteredFields<ListFirmAddressDto>(
                x => x.Address)
            .RegisterFilteredFields<ListFirmContactDto>(
                x => x.Contact)
            .RegisterFilteredFields<ListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.ShortName,
                x => x.LegalAddress,
                x => x.Inn,
                x => x.Kpp,
                x => x.PassportNumber)
            .RegisterFilteredFields<LegalPersonGridDoc>(
                x => x.LegalName,
                x => x.ShortName,
                x => x.LegalAddress,
                x => x.Inn,
                x => x.Kpp,
                x => x.PassportNumber,
                x => x.OwnerName)
            .RegisterFilteredFields<ListLimitDto>(
                x => x.LegalPersonName,
                x => x.ClientName)
            .RegisterFilteredFields<ListLocalMessageDto>(
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListLockDto>(
                x => x.OrderNumber)
            .RegisterFilteredFields<ListLockDetailDto>(
                x => x.Description)
            .RegisterFilteredFields<ListLegalPersonProfileDto>(
                x => x.Name)
            .RegisterFilteredFields<ListOperationDto>(
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListOperationTypeDto>(
                x => x.Name)
            .RegisterFilteredFields<ListOrderPositionDto>(
                x => x.PositionName)
            .RegisterFilteredFields<ListOrderPositionAdvertisementDto>(
                x => x.Id)
            .RegisterFilteredFields<ListOrderDto>(
                x => x.Number,
                x => x.FirmName,
                x => x.SourceOrganizationUnitName,
                x => x.DestOrganizationUnitName,
                x => x.BargainNumber,
                x => x.LegalPersonName)
            .RegisterFilteredFields<OrderGridDoc>(
                x => x.Number,
                x => x.FirmName,
                x => x.SourceOrganizationUnitName,
                x => x.DestOrganizationUnitName,
                x => x.BargainNumber,
                x => x.LegalPersonName,
                x => x.WorkflowStep,
                x => x.OwnerName)
            .RegisterFilteredFields<ListOrderProcessingRequestDto>(
                x => x.Title,
                x => x.BaseOrderNumber,
                x => x.RenewedOrderNumber,
                x => x.FirmName,
                x => x.LegalPersonProfileName,
                x => x.SourceOrganizationUnitName)
            .RegisterFilteredFields<ListOrderFileDto>(
                x => x.FileName)
            .RegisterFilteredFields<ListOrganizationUnitDto>(
                x => x.Name,
                x => x.CountryName)
            .RegisterFilteredFields<ListPlatformDto>(
                x => x.Name)
            .RegisterFilteredFields<ListPositionDto>(
                x => x.Name,
                x => x.PlatformName,
                x => x.CategoryName,
                x => x.ExportCode)
            .RegisterFilteredFields<ListPositionCategoryDto>(
                x => x.Name,
                x => x.ExportCode)
            .RegisterFilteredFields<ListPositionChildrenDto>(
                x => x.Name,
                x => x.PlatformName,
                x => x.CategoryName,
                x => x.ExportCode)
            .RegisterFilteredFields<ListPricePositionDto>(
                x => x.PositionName)
            .RegisterFilteredFields<ListPriceDto>(
                x => x.OrganizationUnitName,
                x => x.CurrencyName)
            .RegisterFilteredFields<ListPrintFormTemplateDto>(
                x => x.FileName)
            .RegisterFilteredFields<ListProjectDto>(
                x => x.DisplayName,
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListReleaseInfoDto>(
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListRoleDto>(
                x => x.Name)
            .RegisterFilteredFields<ListThemeDto>(
                x => x.Name,
                x => x.Description)
            .RegisterFilteredFields<ListThemeTemplateDto>(
                x => x.FileName)
            .RegisterFilteredFields<ListThemeOrganizationUnitDto>(
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListThemeCategoryDto>(
                x => x.CategoryName)
            .RegisterFilteredFields<ListTimeZoneDto>(
                x => x.TimeZoneId)
            .RegisterFilteredFields<ListTerritoryDto>(
                x => x.Name,
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<TerritoryGridDoc>(
                x => x.Name,
                x => x.OrganizationUnitName)
            .RegisterFilteredFields<ListUserDto>(
                x => x.DisplayName,
                x => x.FirstName,
                x => x.LastName,
                x => x.Account)
            .RegisterFilteredFields<UserGridDoc>(
                x => x.DisplayName,
                x => x.FirstName,
                x => x.LastName,
                x => x.Account,
                x => x.ParentName,
                x => x.DepartmentName)
            .RegisterFilteredFields<ListUserRoleDto>(
                x => x.RoleName)
            .RegisterFilteredFields<ListUserOrganizationUnitDto>(
                x => x.OrganizationUnitName,
                x => x.UserName)
            .RegisterFilteredFields<ListUserTerritoryDto>(
                x => x.TerritoryName)
            .RegisterFilteredFields<ListWithdrawalInfoDto>(
                x => x.OrganizationUnitName)
            ;

        private static Dictionary<Type, LambdaExpression[]> RegisterFilteredFields<TDocument>(this Dictionary<Type, LambdaExpression[]> map, params Expression<Func<TDocument, object>>[] expressions)
        {
            var key = typeof(TDocument);
            map.Add(key, expressions);
            return map;
        }

        // может вызываться несколько раз, поэтому есть ContainsKey
        public static void RegisterFilteredFields<TDocument>(params Expression<Func<TDocument, object>>[] expressions)
        {
            var key = typeof(TDocument);

            if (!FilteredFieldsMap.ContainsKey(key))
            {
                FilteredFieldsMap.RegisterFilteredFields(expressions);
            }
        }

        public static bool TryGetFieldFilter<TDocument>(out LambdaExpression[] lambdaExpressions)
        {
            var key = typeof(TDocument);
            return FilteredFieldsMap.TryGetValue(key, out lambdaExpressions);
        }
    }
}