using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Revert;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Revert;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Revert
{
    public class ReopenOperationPhonecallService : IReopenOperationGenericService<Phonecall>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IChangePhonecallStatusAggregateService _changePhonecallStatusAggregateService;

        public ReopenOperationPhonecallService(
            IOperationScopeFactory operationScopeFactory,
            IPhonecallReadModel phonecallReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IChangePhonecallStatusAggregateService changePhonecallStatusAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _phonecallReadModel = phonecallReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _changePhonecallStatusAggregateService = changePhonecallStatusAggregateService;
        }

        public void Revert(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Phonecall>())
            {
                var phonecall = _phonecallReadModel.GetPhonecall(entityId);
                
                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, phonecall.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", phonecall.Header, BLResources.SecurityAccessDenied));
                } 

                _changePhonecallStatusAggregateService.Change(phonecall, ActivityStatus.InProgress);

                scope.Updated<Phonecall>(entityId);
                scope.Complete();
            }
        }
    }
}
