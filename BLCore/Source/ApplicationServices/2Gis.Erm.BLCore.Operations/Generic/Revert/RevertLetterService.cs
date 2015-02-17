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
    public class RevertLetterService : IRevertGenericService<Letter>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ILetterReadModel _letterReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly IChangeLetterStatusAggregateService _changeAppointmentStatusAggregateService;

        public RevertLetterService(
            IOperationScopeFactory operationScopeFactory,
            ILetterReadModel letterReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IChangeLetterStatusAggregateService changeAppointmentStatusAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _letterReadModel = letterReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _changeAppointmentStatusAggregateService = changeAppointmentStatusAggregateService;
        }

        public virtual void Revert(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<RevertIdentity, Letter>())
            {
                var letter = _letterReadModel.GetLetter(entityId);

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, letter.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", letter.Header, BLResources.SecurityAccessDenied));
                }       

                _changeAppointmentStatusAggregateService.Change(letter, ActivityStatus.InProgress);

                scope.Updated<Letter>(entityId);
                scope.Complete();
            }
        }
    }
}
