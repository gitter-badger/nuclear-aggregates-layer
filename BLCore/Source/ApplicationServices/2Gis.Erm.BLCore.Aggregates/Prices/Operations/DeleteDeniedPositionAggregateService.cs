using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class DeleteDeniedPositionAggregateService : IDeleteDeniedPositionAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<DeniedPosition> _deniedPositionRepository;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;

        public DeleteDeniedPositionAggregateService(IOperationScopeFactory operationScopeFactory,
                                                    IRepository<DeniedPosition> deniedPositionRepository,
                                                    IUserContext userContext,
                                                    ISecurityServiceEntityAccess securityServiceEntityAccess)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
            _userContext = userContext;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        public void Delete(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
        {
            CheckDeletePreconditions(deniedPosition, symmetricDeniedPosition);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, DeniedPosition>())
            {
                _deniedPositionRepository.Delete(deniedPosition);
                _deniedPositionRepository.Delete(symmetricDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Deleted(deniedPosition)
                              .Deleted(symmetricDeniedPosition)
                              .Complete();
            }
        }

        public void DeleteSelfDenied(DeniedPosition selfDeniedPosition)
        {
            CheckDeleteSelfDeniedPreconditions(selfDeniedPosition);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, DeniedPosition>())
            {
                _deniedPositionRepository.Delete(selfDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Deleted(selfDeniedPosition)
                              .Complete();
            }
        }

        private void CheckDeletePreconditions(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
        {
            if (deniedPosition.IsSelfDenied())
            {
                throw new NonSelfDeniedPositionExpectedException();
            }

            if (!deniedPosition.IsSymmetricTo(symmetricDeniedPosition))
            {
                throw new SymmetricDeniedPositionExpectedException();
            }

            if (!deniedPosition.IsActive)
            {
                throw new InactiveEntityDeactivationException(typeof(DeniedPosition), deniedPosition.Id);
            }

            if (!symmetricDeniedPosition.IsActive)
            {
                throw new InactiveEntityDeactivationException(typeof(DeniedPosition), symmetricDeniedPosition.Id);
            }

            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Delete,
                                                              EntityName.DeniedPosition,
                                                              _userContext.Identity.Code,
                                                              null,
                                                              0,
                                                              null))
            {
                throw new OperationAccessDeniedException(DeleteIdentity.Instance);
            }
        }

        private void CheckDeleteSelfDeniedPreconditions(DeniedPosition deniedPosition)
        {
            if (!deniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            if (!deniedPosition.IsActive)
            {
                throw new InactiveEntityDeactivationException(typeof(DeniedPosition), deniedPosition.Id);
            }

            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                              EntityName.DeniedPosition,
                                                              _userContext.Identity.Code,
                                                              null,
                                                              0,
                                                              null))
            {
                throw new OperationAccessDeniedException(DeleteIdentity.Instance);
            }
        }
    }
}