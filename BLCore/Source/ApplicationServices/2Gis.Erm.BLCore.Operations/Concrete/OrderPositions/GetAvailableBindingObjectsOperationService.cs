using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
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
                var firmAddresses = GetFirmAddresses(orderDto.FirmId, includeHiddenAddresses);
                var firmAddressesCategories = _categoryReadModel.GetFirmAddressesCategories(orderDto.DestOrganizationUnitId, firmAddresses.Select(x => x.Id));
                foreach (var firmAddress in firmAddresses)
                {
                    firmAddress.Categories = firmAddressesCategories.ContainsKey(firmAddress.Id) ? firmAddressesCategories[firmAddress.Id] : Enumerable.Empty<long>();
                }

                var firmCategoryIds = firmAddresses.SelectMany(firmAddress => firmAddress.Categories).Distinct().ToArray();
                var themeDtos = _themeReadModel.FindThemesCanBeUsedWithOrder(orderDto.DestOrganizationUnitId, orderDto.BeginDistributionDate, orderDto.EndDistributionDatePlan);

                IEnumerable<LinkingObjectsSchemaDto.WarningDto> warnings = null;
                if (firmAddresses.Length == 0)
                {
                    warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.FirmDoesntHaveActiveAddresses } };
                }
                else if (pricePositionInfo.LinkingObjectType == PositionBindingObjectType.ThemeMultiple && !themeDtos.Any())
                {
                    warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.ThereIsNoSuitableThemes } };
                }

                var firmCategories = _categoryReadModel.GetFirmCategories(firmCategoryIds, pricePositionInfo.SalesModel, orderDto.DestOrganizationUnitId);
                var additionalCategories = orderPositionId.HasValue
                                               ? _categoryReadModel.GetAdditionalCategories(firmCategoryIds, orderPositionId.Value, pricePositionInfo.SalesModel, orderDto.DestOrganizationUnitId)
                                               : Enumerable.Empty<LinkingObjectsSchemaDto.CategoryDto>();

                var result = new LinkingObjectsSchemaDto
                           {
                               Warnings = warnings,
                               FirmCategories = firmCategories,
                               AdditionalCategories = additionalCategories,
                               Themes = themeDtos,
                               Positions = _positionReadModel.GetPositionBindingObjectsInfo(pricePositionInfo.IsComposite, pricePositionInfo.PositionId),
                               FirmAddresses = firmAddresses
                           };

                operationScope.Complete();
                return result;
            }
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
    }
}
