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
    public class AssignTaskOperationService : IAssignGenericEntityService<Task>
    {
        private readonly ITaskReadModel _taskReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IActionLogger _actionLogger;
        private readonly IUserReadModel _userReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;
        public AssignTaskOperationService(
            ITaskReadModel taskReadModel,
            IOperationScopeFactory scopeFactory,     
            IActionLogger actionLogger,
            IUserReadModel userReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext, 
            IAssignTaskAggregateService assignTaskAggregateService)
        {
            _taskReadModel = taskReadModel;
            _scopeFactory = scopeFactory;
            _actionLogger = actionLogger;
            _userReadModel = userReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _assignTaskAggregateService = assignTaskAggregateService;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            var entity = _taskReadModel.GetTask(entityId);
            var originalOwner = entity.OwnerCode;

            if (_userReadModel.GetUser(ownerCode).IsServiceUser)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
            }

            if (!_entityAccessService.HasActivityUpdateAccess<Task>(_userContext.Identity, entityId, entity.OwnerCode))
            {
                throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Task>())
            {                
                _assignTaskAggregateService.Assign(entity, ownerCode);

                _actionLogger.LogChanges(entity, x => x.OwnerCode, originalOwner, ownerCode);

                operationScope
                    .Updated<Task>(entityId)
                    .Complete();
                    
                return null;
            }                               
        }
    }
}
