using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignPhonecallOperationService : IAssignGenericEntityService<Phonecall>
    {
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;

        private readonly IActionLogger _actionLogger;

        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;

        public AssignPhonecallOperationService(
            IPhonecallReadModel phonecallReadModel,
            IOperationScopeFactory scopeFactory,
            IUserReadModel userReadModel,
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext, 
            IAssignPhonecallAggregateService assignPhonecallAggregateService)
        {
            _phonecallReadModel = phonecallReadModel;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _actionLogger = actionLogger;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            var entity = _phonecallReadModel.GetPhonecall(entityId);
            var originalOwner = entity.OwnerCode;

            if (_userReadModel.GetUser(ownerCode).IsServiceUser)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
            }

            if (!_entityAccessService.HasActivityUpdateAccess<Phonecall>(_userContext.Identity, entityId, entity.OwnerCode))
            {
                throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Phonecall>())
            {               
                _assignPhonecallAggregateService.Assign(entity, ownerCode);

                _actionLogger.LogChanges(entity, x => x.OwnerCode, originalOwner, ownerCode);

                operationScope
                    .Updated<Phonecall>(entityId)
                    .Complete();
                    
                return null;
            }                   
        }
    }
}
