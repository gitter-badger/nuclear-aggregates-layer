using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Reopen
{
    public class ReopenAppointmentOperationService : IReopenGenericOperationService<Appointment>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;

        private readonly IActionLogger _actionLogger;

        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IReopenAppointmentAggregateService _reopenAppointmentAggregateService;

        public ReopenAppointmentOperationService(
            IOperationScopeFactory operationScopeFactory, 
            IAppointmentReadModel appointmentReadModel,
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IReopenAppointmentAggregateService reopenAppointmentAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _actionLogger = actionLogger;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _reopenAppointmentAggregateService = reopenAppointmentAggregateService;
        }

        public void Reopen(long entityId)
        {           
            using (var scope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Appointment>())
            {
                var appointment = _appointmentReadModel.GetAppointment(entityId);
                var originalStatus = appointment.Status;

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, appointment.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", appointment.Header, BLResources.SecurityAccessDenied));
                }                

                _reopenAppointmentAggregateService.Reopen(appointment);

                _actionLogger.LogChanges(appointment, x => x.Status, originalStatus, ActivityStatus.InProgress);

                scope.Updated<Appointment>(entityId);
                scope.Complete();
            }
        }
    }
}
