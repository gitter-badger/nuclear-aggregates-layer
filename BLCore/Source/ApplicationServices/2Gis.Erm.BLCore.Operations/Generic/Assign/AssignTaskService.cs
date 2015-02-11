using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignTaskService : IAssignGenericEntityService<Task>
    {
        private readonly ITaskReadModel _taskReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;
        public AssignTaskService(
            ITaskReadModel taskReadModel,
            IOperationScopeFactory scopeFactory,     
            IUserReadModel userReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext, 
            IAssignTaskAggregateService assignTaskAggregateService)
        {
            _taskReadModel = taskReadModel;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _assignTaskAggregateService = assignTaskAggregateService;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {           
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Task>())
            {
                var entity = _taskReadModel.GetTask(entityId);
                
                if (_userReadModel.GetUser(ownerCode).IsServiceUser)
                {
                    throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Task>(_userContext.Identity, entityId, entity.OwnerCode))
                {
                    throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
                }

                _assignTaskAggregateService.Assign(entity, ownerCode);

                operationScope
                    .Updated<Task>(entityId)
                    .Complete();
                    
                return null;
            }                               
        }
    }
}
