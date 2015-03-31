using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Reopen
{
    public class ReopenPhonecallOperationService : IReopenGenericOperationService<Phonecall>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IReopenPhonecallAggregateService _reopenPhonecallAggregateService;

        public ReopenPhonecallOperationService(
            IOperationScopeFactory operationScopeFactory,
            IPhonecallReadModel phonecallReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IReopenPhonecallAggregateService reopenPhonecallAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _phonecallReadModel = phonecallReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _reopenPhonecallAggregateService = reopenPhonecallAggregateService;
        }

        public void Reopen(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Phonecall>())
            {
                var phonecall = _phonecallReadModel.GetPhonecall(entityId);
                
                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, phonecall.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", phonecall.Header, BLResources.SecurityAccessDenied));
                } 

                _reopenPhonecallAggregateService.Reopen(phonecall);

                scope.Updated<Phonecall>(entityId);
                scope.Complete();
            }
        }
    }
}
