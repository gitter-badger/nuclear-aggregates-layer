using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Cancel
{
    public class CancelAppointmentOperationService : ICancelGenericOperationService<Appointment>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;

        private readonly IActionLogger _actionLogger;

        private readonly IUserContext _userContext;

        private readonly ICancelAppointmentAggregateService _cancelAppointmentAggregateService;

        public CancelAppointmentOperationService(
            IOperationScopeFactory operationScopeFactory, 
            IAppointmentReadModel appointmentReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IActionLogger actionLogger,
            IUserContext userContext,
            ICancelAppointmentAggregateService cancelAppointmentAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _entityAccessService = entityAccessService;
            _actionLogger = actionLogger;
            _userContext = userContext;
            _cancelAppointmentAggregateService = cancelAppointmentAggregateService;
        }

        public void Cancel(long entityId)
        {                        
            using (var scope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Appointment>())
            {
                var appointment = _appointmentReadModel.GetAppointment(entityId);
                var originalStatus = appointment.Status;

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, appointment.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", appointment.Header, BLResources.SecurityAccessDenied));
                }    

                _cancelAppointmentAggregateService.Cancel(appointment);

                _actionLogger.LogChanges(appointment, x => x.Status, originalStatus, ActivityStatus.Canceled);

                scope.Updated<Appointment>(entityId);
                scope.Complete();
            }
        }
    }
}
