using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnApprovalToRejectedHandler : RequestHandler<ProcessOrderOnApprovalToRejectedRequest, EmptyResponse>
    {
        private readonly INotificationsSettings _notificationsSettings;
        private readonly IUserContext _userContext;
        private readonly INotificationSender _notificationSender;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly ITracer _tracer;
        private readonly IOrderReadModel _orderReadModel;

        public ProcessOrderOnApprovalToRejectedHandler(
            INotificationsSettings notificationsSettings,
            IOrderReadModel orderReadModel,
            IUserContext userContext,
            INotificationSender notificationSender,
            IEmployeeEmailResolver employeeEmailResolver,
            ITracer tracer)
        {
            _notificationsSettings = notificationsSettings;
            _userContext = userContext;
            _notificationSender = notificationSender;
            _employeeEmailResolver = employeeEmailResolver;
            _tracer = tracer;
            _orderReadModel = orderReadModel;
        }

        protected override EmptyResponse Handle(ProcessOrderOnApprovalToRejectedRequest request)
        {
            var order = request.Order;
            if (order == null)
            {
                throw new ArgumentException("Order must be supplied");
            }
            
            NotifyAboutOrderRejected(order);

            return Response.Empty;
        }

        private void NotifyAboutOrderRejected(Order order)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _tracer.Info("Notifications disabled in config file");
                return;
            }

            string orderOwnerEmail;
            if (!_employeeEmailResolver.TryResolveEmail(order.OwnerCode, out orderOwnerEmail) || string.IsNullOrEmpty(orderOwnerEmail))
            {
                _tracer.Error("Can't send notification about - order rejection. Can't get to_address email. Order id: " + order.Id + ". Owner code: " + order.OwnerCode);
                return;
            }

            // ищем последнее примечание к заказу за прошедшие сутки
            var note = _orderReadModel.GetLastNoteForOrder(order.Id, DateTime.UtcNow.AddDays(-1));
            var comment = note != null ? note.Text : BLResources.NotSpecified;

            _notificationSender.PostMessage(new[] { new NotificationAddress(orderOwnerEmail) },
                string.Format(BLResources.OrderStateFromOnApprovalToRejectedSubjectTemplate, order.Number),
                string.Format(BLResources.OrderStateFromOnApprovalToRejectedBodyTemplate, order.Number, _userContext.Identity.DisplayName ?? BLResources.NotDefined, comment));
        }
    }
}
