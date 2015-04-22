using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Territories
{
    public sealed class SelectCurrentUserTerritoriesExpressionHandler : RequestHandler<SelectCurrentUserTerritoriesExpressionRequest, SelectCurrentUserTerritoriesExpressionResponse>
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly IUserContext _userContext;
        private readonly IUserRepository _userRepository;

        public SelectCurrentUserTerritoriesExpressionHandler(
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess, 
            IUserContext userContext, 
            IUserRepository userRepository)   
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _userContext = userContext;
            _userRepository = userRepository;
        }

        protected override SelectCurrentUserTerritoriesExpressionResponse Handle(SelectCurrentUserTerritoriesExpressionRequest request)
        {
            var currentUserOwnerCode = _userContext.Identity.Code;
            var currentUserReserveAccess = GetMaxAccess(_securityServiceFunctionalAccess.GetFunctionalPrivilege(FunctionalPrivilegeName.ReserveAccess, currentUserOwnerCode));
            return new SelectCurrentUserTerritoriesExpressionResponse { TerritoryIds = GetTerritoriesAccordingToAccessLevel(currentUserReserveAccess, currentUserOwnerCode) };
        }

        private IEnumerable<long> GetTerritoriesAccordingToAccessLevel(ReserveAccess accessLevel, long userId)
        {
            switch (accessLevel)
            {
                case ReserveAccess.None:
                    return new long[0];
                case ReserveAccess.Territory:
                    return _userRepository.GetUserTerritoryIds(userId);
                case ReserveAccess.OrganizationUnit:
                    return _userRepository.GetUserOrganizationUnitsTerritoryIds(userId);
                case ReserveAccess.Full:
                    return _userRepository.GetAllTerritoryIds();
                default:
                    throw new NotificationException(String.Format("Уровень доступа пользователя {0} не поддерживается", accessLevel));
            }
        }

        private static ReserveAccess GetMaxAccess(int[] accesses)
        {
            if (!accesses.Any())
                return ReserveAccess.None;

            var priorities = new[] { ReserveAccess.None, ReserveAccess.Territory, ReserveAccess.OrganizationUnit, ReserveAccess.Full };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (ReserveAccess)x)).Max();
            return priorities[maxPriority];
        }
    }
}