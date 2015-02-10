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
    public class AssignLetterService : IAssignGenericEntityService<Letter>
    {
        private readonly ILetterReadModel _letterReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;

        public AssignLetterService(
            ILetterReadModel letterReadModel,
            IOperationScopeFactory scopeFactory,
            IUserReadModel userReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext, 
            IAssignLetterAggregateService assignLetterAggregateService)
        {
            _letterReadModel = letterReadModel;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _assignLetterAggregateService = assignLetterAggregateService;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {           
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Letter>())
            {
                var entity = _letterReadModel.GetLetter(entityId);               

                if (_userReadModel.GetUser(ownerCode).IsServiceUser)
                {
                    throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Letter>(_userContext.Identity, entityId, entity.OwnerCode))
                {
                    throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
                }

                _assignLetterAggregateService.Assign(entity, ownerCode);

                operationScope
                    .Updated<Letter>(entityId)
                    .Complete();
                    
                return null;
            }                   
        }
    } 
}
