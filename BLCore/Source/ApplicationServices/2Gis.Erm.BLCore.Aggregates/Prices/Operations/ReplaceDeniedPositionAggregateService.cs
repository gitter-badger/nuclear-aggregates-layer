using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class ReplaceDeniedPositionAggregateService : IReplaceDeniedPositionAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<DeniedPosition> _deniedPositionRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;

        public ReplaceDeniedPositionAggregateService(IOperationScopeFactory operationScopeFactory,
                                                     IRepository<DeniedPosition> deniedPositionRepository,
                                                     IIdentityProvider identityProvider,
                                                     IUserContext userContext,
                                                     ISecurityServiceEntityAccess securityServiceEntityAccess)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
            _identityProvider = identityProvider;
            _userContext = userContext;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        public void Replace(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition, long positionDeniedId)
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
                throw new OperationAccessDeniedException(ReplaceDeniedPositionIdentity.Instance);
            }

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ReplaceDeniedPositionIdentity>())
            {
                deniedPosition.PositionDeniedId = positionDeniedId;
                _deniedPositionRepository.Update(deniedPosition);
                if (deniedPosition.IsSelfDenied())
                {
                    _deniedPositionRepository.Delete(symmetricDeniedPosition);
                    operationScope.Deleted(symmetricDeniedPosition);
                }
                else
                {
                    symmetricDeniedPosition.PositionId = positionDeniedId;
                    _deniedPositionRepository.Update(symmetricDeniedPosition);
                    operationScope.Updated(symmetricDeniedPosition);
                }

                _deniedPositionRepository.Save();

                operationScope.Updated(deniedPosition)
                              .Complete();
            }
        }

        public void ReplaceSelfDenied(DeniedPosition originalDeniedPosition, long positionDeniedId)
        {
            if (originalDeniedPosition == null)
            {
                throw new ArgumentNullException("originalDeniedPosition");
            }

            if (!originalDeniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ReplaceDeniedPositionIdentity>())
            {
                originalDeniedPosition.PositionDeniedId = positionDeniedId;
                _deniedPositionRepository.Update(originalDeniedPosition);

                if (!originalDeniedPosition.IsSelfDenied())
                {
                    var symmetricDeniedPosition = originalDeniedPosition.CreateSymmetric();

                    _identityProvider.SetFor(symmetricDeniedPosition);
                    _deniedPositionRepository.Add(symmetricDeniedPosition);
                    operationScope.Added(symmetricDeniedPosition);
                }

                _deniedPositionRepository.Save();

                operationScope.Updated(originalDeniedPosition)
                              .Complete();
            }
        }
    }
}