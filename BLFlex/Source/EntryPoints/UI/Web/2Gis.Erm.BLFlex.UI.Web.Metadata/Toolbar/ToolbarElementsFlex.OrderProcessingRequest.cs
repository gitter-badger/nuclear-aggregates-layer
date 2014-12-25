using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar
{
    public sealed partial class ToolbarElementsFlex
    {
        public static class OrderProcessingRequests
        {
            public static UIElementMetadataBuilder CreateOrder()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("CreateOrder")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCreateOrder)
                                     .LockOnNew()
                                     .Handler.Name("scope.CreateOrder")
                                     .ControlType(ControlType.TextButton)
                                     .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                     .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<OrderPosition>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<OrderPositionAdvertisement>(EntityAccessTypes.Create)
                                     .Operation.NonCoupled<CreateOrderByRequestIdentity>();
            }

            public static UIElementMetadataBuilder Cancel()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("CancelOrderProcessingRequest")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCancelOrderProcessingRequest)
                                     .LockOnNew()
                                     .DisableOn<IOrderProcessingRequestViewModel>(x => x.State == OrderProcessingRequestState.Cancelled,
                                                                                  x => x.State == OrderProcessingRequestState.Completed)
                                     .Handler.Name("scope.CancelOrderProcessingRequest")
                                     .ControlType(ControlType.TextButton)
                                     .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                     .Operation.NonCoupled<CancelOrderProcessingRequestIdentity>();
            }
        }
    }
}
