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
    public class RevertAppointmentService : IRevertGenericService<Appointment>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly IChangeAppointmentStatusAggregateService _changeAppointmentStatusAggregateService;

        public RevertAppointmentService(
            IOperationScopeFactory operationScopeFactory, 
            IAppointmentReadModel appointmentReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IChangeAppointmentStatusAggregateService changeAppointmentStatusAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _changeAppointmentStatusAggregateService = changeAppointmentStatusAggregateService;
        }

        public void Revert(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<RevertIdentity, Appointment>())
            {
                var appointment = _appointmentReadModel.GetAppointment(entityId);
                
                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, appointment.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", appointment.Header, BLResources.SecurityAccessDenied));
                }                

                _changeAppointmentStatusAggregateService.Change(appointment, ActivityStatus.InProgress);

                scope.Updated<Appointment>(entityId);
                scope.Complete();
            }
        }
    }
}
