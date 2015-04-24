using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    public sealed class GetAvailableBindingObjectsOperationService : IGetAvailableBindingObjectsOperationService
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IThemeReadModel _themeReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ICategoryReadModel _categoryReadModel;

        public GetAvailableBindingObjectsOperationService(IFirmReadModel firmReadModel,
                                                          IThemeReadModel themeReadModel,
                                                          IPositionReadModel positionReadModel,
                                                          IOrderReadModel orderReadModel,
                                                          IPriceReadModel priceReadModel,
                                                          IOperationScopeFactory operationScopeFactory,
                                                          ICategoryReadModel categoryReadModel)
        {
            _firmReadModel = firmReadModel;
            _themeReadModel = themeReadModel;
            _positionReadModel = positionReadModel;
            _orderReadModel = orderReadModel;
            _priceReadModel = priceReadModel;
            _operationScopeFactory = operationScopeFactory;
            _categoryReadModel = categoryReadModel;
        }

        public LinkingObjectsSchemaDto GetLinkingObjectsSchema(long orderId, long pricePositionId, bool includeHiddenAddresses, long? orderPositionId)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<GetAvailableBinfingObjectsIdentity>())
            {
                var orderDto = _orderReadModel.GetOrderLinkingObjectsDto(orderId);
                var pricePositionInfo = _priceReadModel.GetPricePositionDetailedInfo(pricePositionId);
                var firmAddressDtos = GetFirmAddresses(orderDto.FirmId, includeHiddenAddresses);
                var firmAddressesCategories = _categoryReadModel.GetFirmAddressesCategories(orderDto.DestOrganizationUnitId, firmAddressDtos.Select(x => x.Id));

                var firmCategoryIds = firmAddressesCategories.SelectMany(x => x.Value).Select(x => x.Id).Distinct().ToArray();
                var themeDtos = _themeReadModel.FindThemesCanBeUsedWithOrder(orderDto.DestOrganizationUnitId, orderDto.BeginDistributionDate, orderDto.EndDistributionDatePlan);

                IEnumerable<LinkingObjectsSchemaDto.WarningDto> warnings = null;
                if (firmAddressDtos.Length == 0)
                {
                    warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.FirmDoesntHaveActiveAddresses } };
                }
                else if (pricePositionInfo.Position.BindingObjectTypeEnum == PositionBindingObjectType.ThemeMultiple && !themeDtos.Any())
                {
                    warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.ThereIsNoSuitableThemes } };
                }

                var firmCategoriesSupportedBySalesModel = _categoryReadModel.GetFirmCategories(firmCategoryIds, pricePositionInfo.Position.SalesModel, orderDto.DestOrganizationUnitId);
                var salesIntoCategories = orderPositionId.HasValue
                                              ? _categoryReadModel.GetSalesIntoCategories(orderPositionId.Value)
                                              : Enumerable.Empty<CategoryAsLinkingObjectDto>();

                var salesIntoCategoriesByFirmAddress = salesIntoCategories.Where(x => x.FirmAddressId.HasValue)
                                                                          .GroupBy(x => x.FirmAddressId)
                                                                          .ToDictionary(x => x.Key, y => y);

                // Оставим в качестве допустимых рубрик в адрес только те рубрики, что остались в качестве допустимых рубрик по фирме, либо те, в которые уже были продажи.
                foreach (var firmAddressDto in firmAddressDtos)
                {
                    firmAddressDto.Categories =
                        (firmAddressesCategories.ContainsKey(firmAddressDto.Id)
                             ? firmAddressesCategories[firmAddressDto.Id].Select(x => x.Id).Where(x => firmCategoriesSupportedBySalesModel.Any(y => y.Id == x))
                             : Enumerable.Empty<long>())
                            .Union(salesIntoCategoriesByFirmAddress.ContainsKey(firmAddressDto.Id)
                                       ? salesIntoCategoriesByFirmAddress[firmAddressDto.Id].Select(x => x.CategoryId)
                                       : Enumerable.Empty<long>());
                }

                var positionDtos = _positionReadModel.GetPositionBindingObjectsInfo(pricePositionInfo.Position.IsComposite, pricePositionInfo.Position.Id)
                                                  .Select(ConvertToResponsePositionDto)
                                                  .ToArray();

                if (!pricePositionInfo.Position.IsComposite || pricePositionInfo.Position.AutoCheckSubpositionsWithFirmBindingType())
                {
                    foreach (var positionDto in positionDtos.Where(x => x.LinkingObjectType == PositionBindingObjectType.Firm.ToString()))
                    {
                        positionDto.AlwaysChecked = true;
                    }
                }

                var salesIntoCategoriesByPositions = salesIntoCategories.GroupBy(x => x.PositionId)
                                                                        .ToDictionary(x => x.Key, y => y);

                var allFirmCategories = firmAddressesCategories.SelectMany(x => x.Value).DistinctBy(x => x.Id).ToArray();
                foreach (var position in positionDtos)
                {
                    var salesIntoCategoriesByPosition = salesIntoCategoriesByPositions.ContainsKey(position.Id)
                                                            ? salesIntoCategoriesByPositions[position.Id]
                                                            : Enumerable.Empty<CategoryAsLinkingObjectDto>();

                    position.AvailableCategories = GetCategoriesAvailableForPosition(allFirmCategories,
                                                                                     firmCategoriesSupportedBySalesModel,
                                                                                     salesIntoCategoriesByPosition,
                                                                                     (PositionsGroup)position.PositionsGroup);
                }

                var result = new LinkingObjectsSchemaDto
                                 {
                                     Warnings = warnings,
                                     FirmCategories =
                                         firmCategoriesSupportedBySalesModel.Select(ConvertToResponseCategoryDto)
                                                                            .Concat(salesIntoCategories.Where(AdditionalCategoriesWithSales(firmCategoriesSupportedBySalesModel
                                                                                                                                                .Select(x => x.Id)))
                                                                                                       .Select(LinkingObjectCategoryDto()))
                                                                                                       .ToArray(),
                                     Themes = themeDtos,
                                     Positions = positionDtos,
                                     FirmAddresses = firmAddressDtos
                                 };

                operationScope.Complete();
                return result;
            }
        }

        private static Func<CategoryAsLinkingObjectDto, bool> AdditionalCategoriesWithSales(IEnumerable<long> categories)
        {
            return x => !categories.Contains(x.CategoryId);
        }

        private static Func<CategoryAsLinkingObjectDto, LinkingObjectsSchemaDto.CategoryDto> LinkingObjectCategoryDto()
        {
            return x => new LinkingObjectsSchemaDto.CategoryDto
                            {
                                Id = x.CategoryId,
                                Level = x.CategoryLevel,
                                Name = x.CategoryName
                            };
        }

        private static LinkingObjectsSchemaDto.CategoryDto ConvertToResponseCategoryDto(LinkingObjectsSchemaCategoryDto dto)
        {
            return new LinkingObjectsSchemaDto.CategoryDto
            {
                Id = dto.Id,
                Level = dto.Level,
                Name = dto.Name
            };
        }

        private static LinkingObjectsSchemaDto.PositionDto ConvertToResponsePositionDto(LinkingObjectsSchemaPositionDto dto)
        {
            return new LinkingObjectsSchemaDto.PositionDto
            {
                Id = dto.Id,
                Name = dto.Name,
                PositionsGroup = (int)dto.PositionsGroup,
                LinkingObjectType = dto.BindingObjectType.ToString(),
                IsLinkingObjectOfSingleType = dto.BindingObjectType.IsPositionBindingOfSingleType(),
                AdvertisementTemplateId = dto.AdvertisementTemplateId,
                DummyAdvertisementId = dto.DummyAdvertisementId,
            };
        }

        private LinkingObjectsSchemaDto.FirmAddressDto[] GetFirmAddresses(long firmId, bool includeHiddenAddresses)
        {
            var firmAddresses = includeHiddenAddresses
                                    ? _firmReadModel.GetActiveOrWithSalesByFirm(firmId)
                                    : _firmReadModel.GetFirmAddressesByFirm(firmId);

            return firmAddresses.Select(fa => new LinkingObjectsSchemaDto.FirmAddressDto
                                                  {
                                                      Id = fa.Id,
                                                      Address = FormatAddress(fa.Address, fa.ReferencePoint),
                                                      IsDeleted = fa.IsDeleted || (fa.ClosedForAscertainment && !fa.IsActive),
                                                      IsHidden = fa.ClosedForAscertainment && fa.IsActive && !fa.IsDeleted,
                                                      IsLocatedOnTheMap = fa.IsLocatedOnTheMap,
                                                  })
                                .ToArray();
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

        private IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetCategoriesAvailableForPosition(
            IEnumerable<LinkingObjectsSchemaCategoryDto> allFirmCategories,
            IEnumerable<LinkingObjectsSchemaCategoryDto> supportedBySalesModelCategories,
            IEnumerable<CategoryAsLinkingObjectDto> salesIntoCategoriesByPosition,
            PositionsGroup positionsGroup)
        {
            var positionsGroupCategories = (positionsGroup == PositionsGroup.Media
                                                ? allFirmCategories
                                                : supportedBySalesModelCategories).Select(ConvertToResponseCategoryDto).ToArray();
            return
                positionsGroupCategories.Concat(salesIntoCategoriesByPosition.Where(AdditionalCategoriesWithSales(positionsGroupCategories.Select(x => x.Id)))
                                                                             .Select(LinkingObjectCategoryDto()))
                                        .ToArray();
        }
    }
}