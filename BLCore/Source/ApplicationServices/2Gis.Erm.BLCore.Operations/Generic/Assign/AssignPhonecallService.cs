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

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignPhonecallService : IAssignGenericEntityService<Phonecall>
    {
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;

        public AssignPhonecallService(
            IPhonecallReadModel phonecallReadModel,
            IOperationScopeFactory scopeFactory,
            IUserReadModel userReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext, 
            IAssignPhonecallAggregateService assignPhonecallAggregateService)
        {
            _phonecallReadModel = phonecallReadModel;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {          
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Phonecall>())
            {
                var entity = _phonecallReadModel.GetPhonecall(entityId);
              
                if (_userReadModel.GetUser(ownerCode).IsServiceUser)
                {
                    throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Phonecall>(_userContext.Identity, entityId, entity.OwnerCode))
                {
                    throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
                }

                _assignPhonecallAggregateService.Assign(entity, ownerCode);

                operationScope
                    .Updated<Phonecall>(entityId)
                    .Complete();
                    
                return null;
            }                   
        }
    }
}
