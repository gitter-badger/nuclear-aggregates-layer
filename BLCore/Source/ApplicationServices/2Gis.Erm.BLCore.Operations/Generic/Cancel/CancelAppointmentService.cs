using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Cancel
{
    public class CancelAppointmentService : ICancelGenericService<Appointment>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly IChangeAppointmentStatusAggregateService _changeAppointmentStatusAggregateService;

        public CancelAppointmentService(
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

        public void Cancel(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Appointment>())
            {
                var appointment = _appointmentReadModel.GetAppointment(entityId);

                if (appointment.Status != ActivityStatus.InProgress)
                {
                    throw new BusinessLogicException(string.Format(BLResources.CannotCancelFinishedOrClosedActivity, appointment.Header));
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, appointment.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", appointment.Header, BLResources.SecurityAccessDenied));
                }                

                _changeAppointmentStatusAggregateService.Change(appointment, ActivityStatus.Canceled);

                scope.Updated<Appointment>(entityId);
                scope.Complete();
            }
        }
    }
}
