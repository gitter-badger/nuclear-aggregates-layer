using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public class CreateOrderCreationRequestOperationService : ICreateOrderCreationRequestOperationService
    {
        private const int MinReleaseCountPlan = 4;
        private const int MaxReleaseCountPlan = 12;

        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IOrderProcessingSettings _orderProcessingSettings;
        private readonly IUserReadModel _userReadModel;
        private readonly IFirmRepository _firmRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IProjectService _projectService;
        private readonly ICreatedOrderProcessingRequestEmailSender _emailSender;

        public CreateOrderCreationRequestOperationService(
            IOrderProcessingSettings orderProcessingSettings,
            IUserReadModel userReadModel,
            IOrderProcessingRequestService orderProcessingRequestService,
            IFirmRepository firmRepository,
            ILegalPersonRepository legalPersonRepository,
            ICreatedOrderProcessingRequestEmailSender emailSender,
            IProjectService projectService,
            IUserRepository userRepository,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _orderProcessingSettings = orderProcessingSettings;
            _userReadModel = userReadModel;
            _orderProcessingRequestService = orderProcessingRequestService;
            _firmRepository = firmRepository;
            _legalPersonRepository = legalPersonRepository;
            _projectService = projectService;
            _userRepository = userRepository;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _emailSender = emailSender;
        }

        public long CreateOrderRequest(long sourceProjectCode,
                                       DateTime beginDistributionDate,
                                       short releaseCountPlan,
                                       long firmId,
                                       long legalPersonProfileId,
                                       string description)
        {
            // Значение ReleaseCountPlan должно быть >= 4 и <=12.
            if (releaseCountPlan < MinReleaseCountPlan || releaseCountPlan > MaxReleaseCountPlan)
            {
                throw new BusinessLogicException(string.Format(BLResources.ReleaseCountPlanValueErrorTemplate, MinReleaseCountPlan, MaxReleaseCountPlan));
            }

            var sourceProject = _projectService.GetProjectByCode(sourceProjectCode);
            if (sourceProject == null)
            {
                throw new EntityNotFoundException(typeof(Project), sourceProjectCode);
            }

            if (!sourceProject.OrganizationUnitId.HasValue)
            {
                throw new InvalidOperationException(string.Format(BLResources.ProjectHasNoOrganizationUnit, sourceProject.DisplayName));
            }

            var sourceOrganizationUnitId = sourceProject.OrganizationUnitId.Value;

            var firm = _firmRepository.GetFirm(firmId);
            if (firm == null)
            {
                throw new EntityNotFoundException(typeof(Firm), firmId);
            }

            var reserveUserInfo = _securityServiceUserIdentifier.GetReserveUserIdentity();
            var owner = _userReadModel.GetNotServiceUser(firm.OwnerCode)
                        ?? _userReadModel.GetOrganizationUnitDirector(firm.OrganizationUnitId)
                        ?? _userReadModel.GetUser(reserveUserInfo.Code);

            if (owner == null)
            {
                var message = string.Format(BLResources.ValidOwnerForOrderProcessingRequestMissingErrorTemplate, firmId);
                throw new BusinessLogicException(message);
            }

            var legalPerson = _legalPersonRepository.FindLegalPersonByProfile(legalPersonProfileId);
            if (legalPerson == null)
            {
                throw new EntityNotFoundException(typeof(LegalPersonProfile), legalPersonProfileId);
            }

            // FIXME {all, 03.04.2015}: OrganizationUnit через UserRepository? Да вы издеваетесь, товарищи!
            var sourceOrganizationUnit = _userRepository.GetOrganizationUnit(sourceOrganizationUnitId);
            if (sourceOrganizationUnit == null)
            {
                throw new EntityNotFoundException(typeof(OrganizationUnit), sourceOrganizationUnitId);
            }

            if (!sourceOrganizationUnit.IsActive || sourceOrganizationUnit.IsDeleted)
            {
                throw new InvalidOperationException(string.Format(BLResources.EntityIsInactiveError, typeof(OrganizationUnit).Name, sourceOrganizationUnitId));
            }

            var orderProcessingRequest = new OrderProcessingRequest
                {
                    Description = description,
                    State = OrderProcessingRequestState.Opened,
                    RequestType = OrderProcessingRequestType.CreateOrder,
                    ReplicationCode = Guid.NewGuid(),
                    ReleaseCountPlan = releaseCountPlan,
                    Title = BLResources.NewOrder,
                    DueDate = DateTime.UtcNow.AddHours(_orderProcessingSettings.OrderRequestProcessingHoursAmount),
                    SourceOrganizationUnitId = sourceOrganizationUnitId,
                    FirmId = firmId,
                    LegalPersonProfileId = legalPersonProfileId,
                    LegalPersonId = legalPerson.Id,
                    BeginDistributionDate = beginDistributionDate,
                    OwnerCode = owner.Id,
                    IsDeleted = false,
                    IsActive = true
                };

            _orderProcessingRequestService.Create(orderProcessingRequest);
            var emailSendingResult = _emailSender.SendRequestIsCreatedMessage(orderProcessingRequest);
            if (emailSendingResult.Errors.Any())
            {
                _orderProcessingRequestService.SaveMessagesForOrderProcessingRequest(orderProcessingRequest.Id, GetEmailSendingErrors(emailSendingResult.Errors));
            }

            return orderProcessingRequest.Id;
        }

        private static IEnumerable<IMessageWithType> GetEmailSendingErrors(IEnumerable<string> errors)
        {
            return errors.Select(x => new MessageWithType
                {
                    MessageText = x,
                    Type =  MessageType.Debug
                });
        }
    }
}
