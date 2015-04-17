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
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignLetterOperationService : IAssignGenericEntityService<Letter>
    {
        private readonly ILetterReadModel _letterReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;

        private readonly IActionLogger _actionLogger;

        private readonly IUserContext _userContext;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;

        public AssignLetterOperationService(
            ILetterReadModel letterReadModel,
            IOperationScopeFactory scopeFactory,
            IUserReadModel userReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IActionLogger actionLogger,
            IUserContext userContext, 
            IAssignLetterAggregateService assignLetterAggregateService)
        {
            _letterReadModel = letterReadModel;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _entityAccessService = entityAccessService;
            _actionLogger = actionLogger;
            _userContext = userContext;
            _assignLetterAggregateService = assignLetterAggregateService;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            var entity = _letterReadModel.GetLetter(entityId);
            var originalOwner = entity.OwnerCode;

            if (_userReadModel.GetUser(ownerCode).IsServiceUser)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
            }

            if (!_entityAccessService.HasActivityUpdateAccess<Letter>(_userContext.Identity, entityId, entity.OwnerCode))
            {
                throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Letter>())
            {               
                _assignLetterAggregateService.Assign(entity, ownerCode);

                _actionLogger.LogChanges(entity, x => x.OwnerCode, originalOwner, ownerCode);

                operationScope
                    .Updated<Letter>(entityId)
                    .Complete();
                    
                return null;
            }                   
        }
    } 
}
