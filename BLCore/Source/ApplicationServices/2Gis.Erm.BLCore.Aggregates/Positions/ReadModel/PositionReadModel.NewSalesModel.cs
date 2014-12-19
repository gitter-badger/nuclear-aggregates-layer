using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions.ReadModel
{
    public sealed partial class PositionReadModel
    {
        public bool IsNewSalesModel(PositionAccountingMethod accountingMethod)
        {
            return accountingMethod == PositionAccountingMethod.PlannedProvision;
        }

        public LinkingObjectsSchemaDto GetLinkingObjectsSchema(OrderLinkingObjectsDto dto, PricePositionDetailedInfo pricePositionInfo, bool includeHiddenAddresses, long? orderPositionId)
        {
            var firmAddresses = GetFirmAddresses(dto.FirmId, includeHiddenAddresses, dto.DestOrganizationUnitId);
            var firmCategoryIds = firmAddresses.SelectMany(firmAddress => firmAddress.Categories).Distinct().ToArray();
            var themeDtos = FindThemesCanBeUsedWithOrder(dto);

            IEnumerable<LinkingObjectsSchemaDto.WarningDto> warnings = null;
            if (firmAddresses.Length == 0)
            {
                warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.FirmDoesntHaveActiveAddresses } };
            }
            else if (pricePositionInfo.LinkingObjectType == PositionBindingObjectType.ThemeMultiple && !themeDtos.Any())
            {
                warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.ThereIsNoSuitableThemes } };
            }

            var firmCategories = GetFirmCategories(firmCategoryIds);
            var additionalCategories = GetAdditionalCategories(firmCategoryIds, orderPositionId);
            var categoriesFilter = CreateCategoryFilter(pricePositionInfo.AccountingMethod,
                                                        dto.DestOrganizationUnitId);

            return new LinkingObjectsSchemaDto
            {
                Warnings = warnings,
                FirmCategories = firmCategories.Where(x => categoriesFilter(x.Id)),
                AdditionalCategories = additionalCategories.Where(x => categoriesFilter(x.Id)),
                Themes = themeDtos,
                Positions = GetPositions(pricePositionInfo.IsComposite, pricePositionInfo.PositionId),
                FirmAddresses = firmAddresses
            };
        }

        public IDictionary<long, string> GetNewSalesModelDeniedCategories(PositionAccountingMethod accountingMethod,
                                                                          long destOrganizationUnitId,
                                                                          IEnumerable<long> categoryIds)
        {
            var filter = CreateCategoryFilter(accountingMethod, destOrganizationUnitId);
            var deniedCategories = categoryIds.Where(id => !filter(id));
            return _finder.Find(Specs.Find.ByIds<Category>(deniedCategories))
                          .ToDictionary(category => category.Id, category => category.Name);
        }

        private static bool IsPositionBindingOfSingleType(PositionBindingObjectType type)
        {
            switch (type)
            {
                case PositionBindingObjectType.Firm:
                case PositionBindingObjectType.AddressCategorySingle:
                case PositionBindingObjectType.AddressSingle:
                case PositionBindingObjectType.CategorySingle:
                case PositionBindingObjectType.AddressFirstLevelCategorySingle:
                    return true;
                case PositionBindingObjectType.AddressMultiple:
                case PositionBindingObjectType.CategoryMultiple:
                case PositionBindingObjectType.CategoryMultipleAsterix:
                case PositionBindingObjectType.AddressCategoryMultiple:
                case PositionBindingObjectType.AddressFirstLevelCategoryMultiple:
                case PositionBindingObjectType.ThemeMultiple:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private Func<long, bool> CreateCategoryFilter(PositionAccountingMethod accountingMethod, long destOrganizationUnitId)
        {
            var isNewSalesModel = IsNewSalesModel(accountingMethod);
            if (!isNewSalesModel)
            {
                return categoryId => true;
            }

            if (!NewSalesModelRestrictions.IsOrganizationUnitSupported(destOrganizationUnitId))
            {
                return categoryId => false;
            }

            var supportedCategoryIds = NewSalesModelRestrictions.GetSupportedCategoryIds(destOrganizationUnitId);
            var organizationUnitCategories = _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>() &&
                                                          CategorySpecifications.CategoryOrganizationUnits.Find.ForOrganizationUnit(destOrganizationUnitId) &&
                                                          CategorySpecifications.CategoryOrganizationUnits.Find.ForCategories(supportedCategoryIds))
                                                    .Select(x => x.CategoryId)
                                                    .ToArray();

            return categoryId => organizationUnitCategories.Contains(categoryId);
        }

        private LinkingObjectsSchemaDto.PositionDto[] GetPositions(bool isPricePositionComposite, long positionId)
        {
            var positions = _finder.Find(Specs.Find.ById<Position>(positionId));

            if (isPricePositionComposite)
            {
                positions = positions.SelectMany(x => x.ChildPositions)
                                     .Where(x => !x.IsDeleted)
                                     .Select(x => x.ChildPosition);
            }

            return positions.Select(x => new
            {
                x.Id,
                x.Name,
                x.BindingObjectTypeEnum,
                x.AdvertisementTemplateId,
                x.AdvertisementTemplate.DummyAdvertisementId
            })
                            .ToArray()
                            .Select(x => new LinkingObjectsSchemaDto.PositionDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                LinkingObjectType = (x.BindingObjectTypeEnum).ToString(),
                                AdvertisementTemplateId = x.AdvertisementTemplateId,
                                DummyAdvertisementId = x.DummyAdvertisementId,
                                IsLinkingObjectOfSingleType = IsPositionBindingOfSingleType(x.BindingObjectTypeEnum)
                            })
                            .ToArray();
        }

        private LinkingObjectsSchemaDto.CategoryDto[] GetFirmCategories(IEnumerable<long> firmCategoryIds)
        {
            return _finder.Find(Specs.Find.ByIds<Category>(firmCategoryIds))
                          .Select(item => new LinkingObjectsSchemaDto.CategoryDto { Id = item.Id, Name = item.Name, Level = item.Level, })
                          .Distinct()
                          .ToArray();
        }

        private LinkingObjectsSchemaDto.CategoryDto[] GetAdditionalCategories(IEnumerable<long> firmCategoryIds, long? orderPositionId)
        {
            if (orderPositionId == null)
            {
                return new LinkingObjectsSchemaDto.CategoryDto[0];
            }

            return _finder.Find(OrderSpecs.OrderPositionAdvertisements.Find.ByOrderPosition(orderPositionId.Value))
                          .Where(opa => opa.CategoryId.HasValue)
                          .Select(opa => opa.Category)
                          .Where(category => !firmCategoryIds.Contains(category.Id))
                          .Select(category => new LinkingObjectsSchemaDto.CategoryDto { Id = category.Id, Name = category.Name, Level = category.Level, })
                          .Distinct()
                          .ToArray();
        }

        private LinkingObjectsSchemaDto.FirmAddressDto[] GetFirmAddresses(long firmId, bool includeHiddenAddresses, long destOrganizationUnitId)
        {
            var firmAddressSpecification = includeHiddenAddresses
                ? FirmSpecs.Addresses.Find.ActiveOrWithSalesByFirmId(firmId)
                : FirmSpecs.Addresses.Find.ActiveByFirmId(firmId);

            var firmAddresses =
                _finder.Find(firmAddressSpecification)
                       .Select(address => new
                       {
                           address.Id,
                           address.Address,
                           address.ReferencePoint,
                           IsDeleted = address.IsDeleted || (address.ClosedForAscertainment && !address.IsActive),
                           IsHidden = address.ClosedForAscertainment && address.IsActive && !address.IsDeleted,
                           address.IsLocatedOnTheMap,
                       })
                       .ToArray();

            return firmAddresses.Select(fa => new LinkingObjectsSchemaDto.FirmAddressDto
            {
                Id = fa.Id,
                Address = FormatAddress(fa.Address, fa.ReferencePoint),
                IsDeleted = fa.IsDeleted,
                IsHidden = fa.IsHidden,
                IsLocatedOnTheMap = fa.IsLocatedOnTheMap,
                Categories = GetFirmAddressCategories(fa.Id, destOrganizationUnitId),
            })
                                .ToArray();
        }

        private long[] GetFirmAddressCategories(long firmAddressId, long destOrganizationUnitId)
        {
            var categoryOrganizationUnits = _finder.Find<CategoryOrganizationUnit>(link => link.OrganizationUnitId == destOrganizationUnitId && link.IsActive && !link.IsDeleted);
            var categoryFirmAddress = _finder.Find<CategoryFirmAddress>(link => link.FirmAddressId == firmAddressId && link.IsActive && !link.IsDeleted);

            var directLinkedCategories = categoryFirmAddress.Join(categoryOrganizationUnits,
                                                                  categoryAddress => categoryAddress.CategoryId,
                                                                  categoryOrganizationUnit => categoryOrganizationUnit.CategoryId,
                                                                  (x, y) => x.Category)
                                                            .Where(category => category.IsActive && !category.IsDeleted);

            var firstLevelCategories = directLinkedCategories.Where(category => category.Level == 3)
                                                             .Select(category => category.ParentCategory.ParentCategory)
                                                             .Where(category => category.IsActive && !category.IsDeleted);

            return directLinkedCategories.Union(firstLevelCategories).Select(category => category.Id).ToArray();
        }

        private string FormatAddress(string address, string referencePoint)
        {
            if (string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(referencePoint))
            {
                return BLResources.ViewOrderPositionHandler_EmptyAddress;
            }

            if (string.IsNullOrWhiteSpace(referencePoint))
            {
                return address;
            }

            return string.Format("{0} — {1}", address, referencePoint);
        }

        private LinkingObjectsSchemaDto.ThemeDto[] FindThemesCanBeUsedWithOrder(OrderLinkingObjectsDto dto)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(dto.DestOrganizationUnitId))
                          .SelectMany(unit => unit.ThemeOrganizationUnits)
                          .Where(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                          .Select(link => link.Theme)
                          .Where(Specs.Find.ActiveAndNotDeleted<Theme>())
                          .Where(theme => theme.BeginDistribution <= dto.BeginDistributionDate
                                          && theme.EndDistribution >= dto.EndDistributionDatePlan
                                          && !theme.IsDefault
                                          && !theme.ThemeTemplate.IsSkyScraper)
                          .Select(theme => new LinkingObjectsSchemaDto.ThemeDto
                          {
                              Id = theme.Id,
                              Name = theme.Name
                          })
                          .ToArray();
        }
    }
}