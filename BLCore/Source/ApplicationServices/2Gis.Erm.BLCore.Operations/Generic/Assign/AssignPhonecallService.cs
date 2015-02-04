using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignPhonecallService:IAssignGenericEntityService<Phonecall>
    {
        private readonly IFinder _finder;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;

        public AssignPhonecallService(
            IFinder finder,
            IOperationScopeFactory scopeFactory,
            IUserReadModel userReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext, 
            IAssignPhonecallAggregateService assignPhonecallAggregateService)
        {
            _finder = finder;
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
                var entity = _finder.FindOne(Specs.Find.ById<Phonecall>(entityId));

                if (!_userContext.Identity.SkipEntityAccessCheck)
                {
                    var ownerCanBeChanged = _entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                                                 EntityName.Phonecall,
                                                                                 _userContext.Identity.Code,
                                                                                 entityId,
                                                                                 entity.OwnerCode,
                                                                                 null);
                    if (!ownerCanBeChanged)
                    {
                        throw new SecurityException(string.Format(BLResources.AssignActivityAccessDenied, entity.Header));
                    }
                }

                if (entity.Status != ActivityStatus.InProgress)
                    throw new BusinessLogicException(BLResources.CannotAssignActivityNotInProgress);

                if (_userReadModel.GetUser(ownerCode).IsServiceUser)
                    throw new BusinessLogicException(BLResources.CannotAssignActivitySystemUser);
                   
                _assignPhonecallAggregateService.Assign(entity,ownerCode);

                operationScope
                    .Updated<Phonecall>(entityId)
                    .Complete();
                    
                return null;
            }                   
        }
    }
}
