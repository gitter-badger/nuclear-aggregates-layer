using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

// COMMENT {d.ivanov, 03.06.2014}: По-хорошему - эта штука должна лежать в BLFlex, и еще много чего должно быть зафлексино касательно позиции заказа. 
// C учетом компромисса, на который мы пошли, этот сервис будет лежать в Core и мы обойдемся только разными вариантами вьюх и тем, что Ндс в Dubai будет = 0.
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOrderPositionDtoService : GetDomainEntityDtoServiceBase<OrderPosition>
    {
        private readonly ISecureFinder _finder;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IReleaseReadModel _releaseReadModel;

        public GetOrderPositionDtoService(IUserContext userContext,
                                          ISecureFinder finder,
                                          ISecurityServiceUserIdentifier userIdentifierService,
                                          IReleaseReadModel releaseReadModel)
            : base(userContext)
        {
            _finder = finder;
            _userIdentifierService = userIdentifierService;
            _releaseReadModel = releaseReadModel;
        }

        protected override IDomainEntityDto<OrderPosition> GetDto(long entityId)
        {
            var dto = _finder.FindObsolete(new FindSpecification<OrderPosition>(x => x.Id == entityId))
                          .Select(x => new OrderPositionDomainEntityDto
                              {
                                  Id = x.Id,
                                  OrderId = x.OrderId,
                                  OrderNumber = x.Order.Number,
                                  IsComposite = x.PricePosition.Position.IsComposite,
                                  OrganizationUnitId = x.Order.DestOrganizationUnitId,
                                  PeriodStartDate = x.Order.BeginDistributionDate,
                                  PeriodEndDate = x.Order.EndDistributionDateFact,
                                  Amount = x.Amount,
                                  PricePerUnit = x.PricePerUnit,
                                  PricePerUnitWithVat = x.PricePerUnitWithVat,
                                  DiscountPercent = x.DiscountPercent,
                                  DiscountSum = x.DiscountSum,
                                  PayablePrice = x.PayablePrice,
                                  PayablePlan = x.PayablePlan,
                                  ShipmentPlan = x.ShipmentPlan,
                                  Comment = x.Comment,
                                  CalculateDiscountViaPercent = x.CalculateDiscountViaPercent,
                                  PricePositionRef = new EntityReference { Id = x.PricePositionId, Name = x.PricePosition.Position.Name },
                                  Advertisements = x.OrderPositionAdvertisements
                                                    .Select(y => new AdvertisementDescriptor
                                                        {
                                                            PositionId = y.PositionId,
                                                            AdvertisementId = y.AdvertisementId,
                                                            AdvertisementName = y.Advertisement.Name,
                                                            CategoryId = y.CategoryId,
                                                            CategoryName = y.Category.Name,
                                                            FirmAddressId = y.FirmAddressId,
                                                            FirmAddress = y.FirmAddress.Address + (y.FirmAddress.ReferencePoint == null
                                                                                                       ? string.Empty
                                                                                                       : " — " + y.FirmAddress.ReferencePoint),
                                                            ThemeId = y.ThemeId,
                                                            ThemeName = y.Theme.Name,
                                                        }),
                                  OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn,
                                  Timestamp = x.Timestamp,
                                  CategoryRate = x.CategoryRate
                              })
                          .Single();

            dto.OwnerRef.Name = _userIdentifierService.GetUserInfo(dto.OwnerRef.Id).DisplayName;

            return dto;
        }

        protected override IDomainEntityDto<OrderPosition> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            if (!parentEntityId.HasValue)
            {
                throw new ArgumentNullException("parentEntityId");
            }

            return new OrderPositionDomainEntityDto
                       {
                           CalculateDiscountViaPercent = true,
                           OrderId = parentEntityId.Value,
                           Advertisements = Enumerable.Empty<AdvertisementDescriptor>(),
                       };
        }

        protected override void SetDtoProperties(IDomainEntityDto<OrderPosition> domainEntityDto, long entityId, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var modelDto = (OrderPositionDomainEntityDto)domainEntityDto;

            var minimumPublishDate = DateTime.Now.AddDays(1).Date;
            var orderInfo = _finder.FindObsolete(new FindSpecification<Order>(x => x.Id == modelDto.OrderId))
                                   .Select(x => new
                                       {
                                           x.WorkflowStepId,
                                           x.DestOrganizationUnitId,
                                           x.BeginDistributionDate,
                                           x.EndDistributionDateFact,
                                           DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                                           x.FirmId,
                                           PriceId = x.DestOrganizationUnit.Prices
                                                      .Where(y => y.IsPublished &&
                                                                  y.IsActive && !y.IsDeleted &&
                                                                  y.BeginDate <= x.BeginDistributionDate &&
                                                                  y.PublishDate <= minimumPublishDate)
                                                      .OrderBy(y => y.BeginDate)
                                                      .Select(y => y.Id),
                                           OrderPositionCount = x.OrderPositions.Count(y => y.IsActive && !y.IsDeleted),
                                           x.PlatformId
                                       })
                                   .Single();

            if (orderInfo.WorkflowStepId != OrderState.OnRegistration && modelDto.Id == 0)
            {
                throw new NotificationException(BLResources.CannotCreateOrderPositionWhenOrderIsNotOnRegistration);
            }

            if (!orderInfo.PriceId.Any())
            {
                throw new NotificationException(string.Format(BLResources.PriceForOrganizationUnitNotExists, orderInfo.DestOrganizationUnitName));
            }

            modelDto.PriceId = orderInfo.PriceId.Last();
            modelDto.OrganizationUnitId = orderInfo.DestOrganizationUnitId;
            modelDto.OrderFirmId = orderInfo.FirmId;
            modelDto.RequiredPlatformId = orderInfo.OrderPositionCount > (modelDto.Id == 0 ? 0 : 1) ? orderInfo.PlatformId : null;

            // Сборка в статусе "InProgress" за период, который пересекается с периодом размещения заказа или заказ в Архиве 
            if (orderInfo.WorkflowStepId == OrderState.Approved || orderInfo.WorkflowStepId == OrderState.OnTermination)
            {
                modelDto.IsBlockedByRelease = _releaseReadModel.HasFinalReleaseInProgress(orderInfo.DestOrganizationUnitId,
                                                                                          new TimePeriod(orderInfo.BeginDistributionDate, orderInfo.EndDistributionDateFact));
            }

            modelDto.OrderWorkflowStepId = (int)orderInfo.WorkflowStepId;
            modelDto.IsRated = modelDto.PricePositionRef != null &&
                               _finder.FindObsolete(new FindSpecification<PricePosition>(x => x.Id == modelDto.PricePositionRef.Id)).Select(x => x.RateType != 0).Single();
        }
    }
}