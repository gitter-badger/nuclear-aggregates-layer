using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class ChangeDeniedPositionObjectBindingTypeAggregateService : IChangeDeniedPositionObjectBindingTypeAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<DeniedPosition> _deniedPositionRepository;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;

        public ChangeDeniedPositionObjectBindingTypeAggregateService(IOperationScopeFactory operationScopeFactory,
                                                                     IRepository<DeniedPosition> deniedPositionRepository,
                                                                     IUserContext userContext,
                                                                     ISecurityServiceEntityAccess securityServiceEntityAccess)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
            _userContext = userContext;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        public void Change(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition, ObjectBindingType newObjectBindingType)
        {
            if (deniedPosition == null)
            {
                throw new ArgumentNullException("deniedPosition");
            }

            if (symmetricDeniedPosition == null)
            {
                throw new ArgumentNullException("symmetricDeniedPosition");
            }

            if (deniedPosition.IsSelfDenied())
            {
                throw new NonSelfDeniedPositionExpectedException();
            }

            if (!deniedPosition.IsSymmetricTo(symmetricDeniedPosition))
            {
                throw new SymmetricDeniedPositionExpectedException();
            }

            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                              EntityName.DeniedPosition,
                                                              _userContext.Identity.Code,
                                                              deniedPosition.Id,
                                                              0,
                                                              null))
            {
                throw new OperationAccessDeniedException(ChangeDeniedPositionObjectBindingTypeIdentity.Instance);
            }

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ChangeDeniedPositionObjectBindingTypeIdentity>())
            {
                deniedPosition.ObjectBindingType = newObjectBindingType;
                symmetricDeniedPosition.ObjectBindingType = newObjectBindingType;

                _deniedPositionRepository.Update(deniedPosition);
                _deniedPositionRepository.Update(symmetricDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(deniedPosition)
                              .Updated(symmetricDeniedPosition)
                              .Complete();
            }
        }

        public void ChangeSelfDenied(DeniedPosition selfDeniedPosition, ObjectBindingType newObjectBindingType)
        {
            if (selfDeniedPosition == null)
            {
                throw new ArgumentNullException("selfDeniedPosition");
            }

            if (!selfDeniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ChangeDeniedPositionObjectBindingTypeIdentity>())
            {
                selfDeniedPosition.ObjectBindingType = newObjectBindingType;
                _deniedPositionRepository.Update(selfDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(selfDeniedPosition)
                              .Complete();
            }
        }
    }
}