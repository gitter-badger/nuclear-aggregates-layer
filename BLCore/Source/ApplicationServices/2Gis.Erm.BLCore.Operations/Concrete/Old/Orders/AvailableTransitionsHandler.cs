using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class AvailableTransitionsHandler : RequestHandler<AvailableTransitionsRequest, AvailableTransitionsResponse>
    {
        private readonly IUserContext _userContext;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;

        public AvailableTransitionsHandler(
            IUserContext userContext,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess)
        {
            _userContext = userContext;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
        }

        protected override AvailableTransitionsResponse Handle(AvailableTransitionsRequest request)
        {
            var transitions = OrderStateBehaviourFactory.GetTransitionsForUi(request.CurrentState);
            transitions = FilterStates(request.OrderId, request.SourceOrganizationUnitId, request.CurrentState, transitions.ToList());            
            return new AvailableTransitionsResponse(transitions);
        }

        private IEnumerable<OrderState> FilterStates(long orderId, long? sourceOrganizationUnitId, OrderState currentState, IList<OrderState> availableSteps)
        {
            var currentUserCode = _userContext.Identity.Code;
            switch (currentState)
            {
                case OrderState.OnApproval:
                    if (!_securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderStatesAccess, currentUserCode))
                    {
                        availableSteps.Remove(OrderState.Rejected);
                        availableSteps.Remove(OrderState.Approved);
                    }
                    break;
                case OrderState.Approved:
                    var hasLocks = _accountRepository.IsNonDeletedLocksExists(orderId);
                    if (hasLocks)
                    {
                        availableSteps.Remove(OrderState.OnRegistration);
                    }
                    if (!_securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderStatesAccess, currentUserCode))
                    {
                        availableSteps.Remove(OrderState.OnTermination);
                    }
                    break;
                case OrderState.OnTermination:
                    if (!_securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderStatesAccess, currentUserCode))
                    {
                        availableSteps.Remove(OrderState.Approved);
                        availableSteps.Remove(OrderState.Archive);
                    }
                    break;
            }

            // Заказ должен принадлежать отделению организации, привязанному к пользователю
            var canChangeStates = true;
            if (sourceOrganizationUnitId.HasValue)
            {
                canChangeStates = _userRepository.IsUserLinkedWithOrganizationUnit(currentUserCode, sourceOrganizationUnitId.Value);
            }

            return canChangeStates ? availableSteps : Enumerable.Empty<OrderState>();
        }
    }
}
