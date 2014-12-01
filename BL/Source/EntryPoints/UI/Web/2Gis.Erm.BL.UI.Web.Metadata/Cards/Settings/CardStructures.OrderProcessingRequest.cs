using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata OrderProcessingRequest =
            CardMetadata.For<OrderProcessingRequest>()
                        .MainAttribute<OrderProcessingRequest, IOrderProcessingRequestViewModel>(x => x.Title)
                        .Actions.Attach(UiElementMetadata.Config.RefreshAction<OrderProcessingRequest>(),
                                        UiElementMetadata.Config.AdditionalActions(
                                            UiElementMetadata.Config
                                                             .Name.Static("CreateOrder")
                                                             .Title.Resource(() => ErmConfigLocalization.ControlCreateOrder)
                                                             .LockOnNew()
                                                             .Handler.Name("scope.CreateOrder")
                                                             .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                                             .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                                             .AccessWithPrivelege<OrderPosition>(EntityAccessTypes.Create)
                                                             .AccessWithPrivelege<OrderPositionAdvertisement>(EntityAccessTypes.Create)
                                                             .Operation.NonCoupled<CreateOrderByRequestIdentity>(),
                                            UiElementMetadata.Config
                                                             .Name.Static("CancelOrderProcessingRequest")
                                                             .Title.Resource(() => ErmConfigLocalization.ControlCancelOrderProcessingRequest)
                                                             .LockOnNew()
                                                             .Handler.Name("scope.CancelOrderProcessingRequest")
                                                             .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                                             .Operation.NonCoupled<CancelOrderProcessingRequestIdentity>()),
                                        UiElementMetadata.Config.CloseAction());
    }
}