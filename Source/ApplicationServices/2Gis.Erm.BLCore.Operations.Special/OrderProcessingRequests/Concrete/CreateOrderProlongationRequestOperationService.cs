using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
    public class CreateOrderProlongationRequestOperationService : ICreateOrderProlongationRequestOperationService
    {
        private const int MinReleaseCountPlan = 4;
        private const int MaxReleaseCountPlan = 12;

        private static readonly OrderState[] InvalidOrderStates = { OrderState.OnRegistration, OrderState.Rejected, OrderState.OnApproval };

        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly ISecureFinder _secureFinder;
        private readonly IOrderProcessingRequestOwnerSelectionService _userSelectionService;
        private readonly IAppSettings _appSettings;

        public CreateOrderProlongationRequestOperationService(
            IAppSettings settings,
            ISecureFinder secureFinder,
            IOrderProcessingRequestService orderProcessingRequestService,
            IOrderProcessingRequestOwnerSelectionService userSelectionService)
        {
            _appSettings = settings;
            _secureFinder = secureFinder;
            _orderProcessingRequestService = orderProcessingRequestService;
            _userSelectionService = userSelectionService;
        }

        public long CreateOrderProlongationRequest(long orderId, short releaseCountPlan, string description)
        {
            var orderToProlongate = _secureFinder.Find(Specs.Find.ById<Order>(orderId)).SingleOrDefault();
            if (orderToProlongate == null)
            {
                throw new EntityNotFoundException(typeof(Order), orderId);
            }

            var orderState = (OrderState)orderToProlongate.WorkflowStepId;
            if (InvalidOrderStates.Contains(orderState))
            {
                throw new BusinessLogicException(string.Format(BLResources.CantCreateProlongationRequestInvalidOrderState,
                                                               orderState.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));
            }

            // Значение ReleaseCountPlan должно быть >= 4 и <=12.
            if (releaseCountPlan < MinReleaseCountPlan || releaseCountPlan > MaxReleaseCountPlan)
            {
                throw new BusinessLogicException(string.Format(BLResources.ReleaseCountPlanValueErrorTemplate, MinReleaseCountPlan, MaxReleaseCountPlan));
            }

            var legalPersonProfileId = orderToProlongate.LegalPersonProfileId ?? GetMainLegalPersonProfile(orderToProlongate.LegalPersonId.GetValueOrDefault());

            if (legalPersonProfileId == null)
            {
                throw new BusinessLogicException(BLResources.LegalPersonProfileMissing);
            }

            // FIXME {y.baranihin, 29.11.2013}: похоже на DateTimeExtensions.GetNextMonthFirstDate, можно её использовать? и внутри условия.
            var beginDistributionDate =
                new DateTime(orderToProlongate.EndDistributionDateFact.Year, orderToProlongate.EndDistributionDateFact.Month, 1).AddMonths(1);
            if (DateTime.Today >= beginDistributionDate)
            {
                beginDistributionDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1);
            }

            var owner = _userSelectionService.GetOwner(orderToProlongate.OwnerCode)
                        ?? _userSelectionService.GetOrganizationUnitDirector(orderToProlongate.DestOrganizationUnitId)
                        ?? _userSelectionService.GetReserveUser();

            if (owner == null)
            {
                var message = string.Format("Не найден пользователь, которому следует назначить заявку на продление заказа Order.Id {0}", orderId);
                throw new BusinessLogicException(message);
            }

            var orderProcessingRequestDomainEntityDto = new OrderProcessingRequest
            {
                BaseOrderId = orderId,
                Description = description,
                State = (int)OrderProcessingRequestState.Opened,
                RequestType = (int)OrderProcessingRequestType.ProlongateOrder,
                ReleaseCountPlan = releaseCountPlan,
                ReplicationCode = Guid.NewGuid(),
                Title = BLResources.OrderProlongation,
                DueDate = DateTime.UtcNow.AddHours(_appSettings.OrderRequestProcessingHoursAmount),
                SourceOrganizationUnitId = orderToProlongate.SourceOrganizationUnitId,
                FirmId = orderToProlongate.FirmId,
                LegalPersonProfileId = legalPersonProfileId.Value,
                LegalPersonId = orderToProlongate.LegalPersonId.GetValueOrDefault(),
                OwnerCode = owner.Id,
                BeginDistributionDate = beginDistributionDate,
                IsActive = true,
                IsDeleted = false
            };

            _orderProcessingRequestService.Create(orderProcessingRequestDomainEntityDto);
            return orderProcessingRequestDomainEntityDto.Id;
        }

        private long? GetMainLegalPersonProfile(long legalPersonId)
        {
            var mainLegalPersonProfile = _secureFinder
                .Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId))
                .FirstOrDefault();

            return mainLegalPersonProfile == null ? null : (long?)mainLegalPersonProfile.Id;
        }
    }
}
