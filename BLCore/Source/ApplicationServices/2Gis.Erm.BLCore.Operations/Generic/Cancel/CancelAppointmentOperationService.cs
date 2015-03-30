using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Cancel
{
    public class CancelAppointmentOperationService : ICancelGenericOperationService<Appointment>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly ICancelAppointmentAggregateService _cancelAppointmentAggregateService;

        public CancelAppointmentOperationService(
            IOperationScopeFactory operationScopeFactory, 
            IAppointmentReadModel appointmentReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICancelAppointmentAggregateService cancelAppointmentAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _cancelAppointmentAggregateService = cancelAppointmentAggregateService;
        }

        public void Cancel(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Appointment>())
            {
                var appointment = _appointmentReadModel.GetAppointment(entityId);               

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, appointment.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", appointment.Header, BLResources.SecurityAccessDenied));
                }                

                _cancelAppointmentAggregateService.Cancel(appointment);

                scope.Updated<Appointment>(entityId);
                scope.Complete();
            }
        }
    }
}
