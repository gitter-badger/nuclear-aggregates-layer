using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // FIXME {all, 13.11.2013}: используется Finder, что не допустимо для OperationService
    public class CreateOrderProlongationRequestOperationService : ICreateOrderProlongationRequestOperationService
    {
        private const int MinReleaseCountPlan = 4;
        private const int MaxReleaseCountPlan = 12;

        private static readonly OrderState[] InvalidOrderStates = { OrderState.OnRegistration, OrderState.Rejected, OrderState.OnApproval };

        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly ISecureFinder _secureFinder;
        private readonly IOrderProcessingSettings _orderProcessingSettings;
        private readonly IUserReadModel _userReadModel;

        public CreateOrderProlongationRequestOperationService(
            IOrderProcessingSettings orderProcessingSettings,
            IUserReadModel userReadModel,
            ISecureFinder secureFinder,
            IOrderProcessingRequestService orderProcessingRequestService,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _orderProcessingSettings = orderProcessingSettings;
            _userReadModel = userReadModel;
            _secureFinder = secureFinder;
            _orderProcessingRequestService = orderProcessingRequestService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
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

            var reserveUserInfo = _securityServiceUserIdentifier.GetReserveUserIdentity();
            var owner = _userReadModel.GetNotServiceUser(orderToProlongate.OwnerCode)
                        ?? _userReadModel.GetOrganizationUnitDirector(orderToProlongate.DestOrganizationUnitId)
                        ?? _userReadModel.GetUser(reserveUserInfo.Code);

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
                DueDate = DateTime.UtcNow.AddHours(_orderProcessingSettings.OrderRequestProcessingHoursAmount),
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
            var mainLegalPersonProfile = _secureFinder.FindMany(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId))
                                                      .FirstOrDefault();

            return mainLegalPersonProfile == null ? null : (long?)mainLegalPersonProfile.Id;
        }
    }
}
