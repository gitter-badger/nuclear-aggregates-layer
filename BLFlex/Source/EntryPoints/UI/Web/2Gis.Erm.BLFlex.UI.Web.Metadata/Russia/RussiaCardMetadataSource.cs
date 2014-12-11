using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Russia
{
    public sealed class RussiaCardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>, IRussiaAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public RussiaCardMetadataSource()
        {
            _metadata = InitializeMetadataContainer();
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private static IReadOnlyDictionary<Uri, IMetadataElement> InitializeMetadataContainer()
        {
            IReadOnlyCollection<CardMetadata> metadataContainer =
                new CardMetadata[]
                    {
                        CardMetadata.For<Deal>()
                                    .MVVM.Bind<IDealViewModel>("~/Views/CreateOrUpdate/Russia/Deal.cshtml"),

                        CardMetadata.For<OrderPosition>()
                                    .MVVM.Bind<IOrderPositionViewModel>("~/Views/CreateOrUpdate/Russia/OrderPosition.cshtml"),

                        CardMetadata.For<AdvertisementElementStatus>()
                                    .MainAttribute(x => x.Id),

                        #region OrderProcessingRequest
                        CardMetadata.For<OrderProcessingRequest>()
                                    .MainAttribute<OrderProcessingRequest, IOrderProcessingRequestViewModel>(x => x.Title)
                                    .Actions.Attach(UiElementMetadata.Config.RefreshAction<OrderProcessingRequest>(),
                                                    UiElementMetadata.Config.AdditionalActions
                                                        (UiElementMetadata.Config
                                                                          .Name.Static("CreateOrder")
                                                                          .Title.Resource(() => ErmConfigLocalization.ControlCreateOrder)
                                                                          .LockOnNew()
                                                                          .Handler.Name("scope.CreateOrder")
                                                                          .ControlType(ControlType.TextButton)
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
                                                                          .ControlType(ControlType.TextButton)
                                                                          .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                                                          .Operation.NonCoupled<CancelOrderProcessingRequestIdentity>()),
                                                    UiElementMetadata.Config.CloseAction()),

                        #endregion

                        #region LegalPerson
                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UiElementMetadata.Config.CommonLegalPersonAdditionalActions()
                                                                                                            .With(UiElementMetadata.Config.MergeLegalPersonsAction())),

                        #endregion

                        #region Order
                        CardMetadata.For<Order>()
                                    .ConfigOrderToolbarWithSpecificPrintActions(UiElementMetadata.Config.RussianOrderPrintActions())
                                    .ConfigRelatedItems(UiElementMetadata.Config.CommonOrderRelatedActions()
                                                                         .With(UiElementMetadata.Config.Name.Static("OrderProcessingRequests")
                                                                                                .Title.Resource(() => ErmConfigLocalization.CrdRelOrderProcessingRequests)
                                                                                                .LockOnNew()
                                                                                                .Handler.ShowGridByConvention(EntityName.OrderProcessingRequest)
                                                                                                .FilterToParent())),

                        #endregion

                        #region Bargain
                        CardMetadata.For<Bargain>()
                                    .ConfigBargainToolbarWithSpecificPrintActions(UiElementMetadata.Config.PrintBargainAction(),
                                                                                  UiElementMetadata.Config.PrintNewSalesModelBargainAction(),
                                                                                  UiElementMetadata.Config.PrintBargainProlongationAgreementAction()),

                        #endregion

                        #region Advertisement
                        CardMetadata.For<Advertisement>()
                                    .Actions
                                    .Attach(UiElementMetadata.Config.CreateAction<Advertisement>(),
                                            UiElementMetadata.Config.UpdateAction<Advertisement>(),
                                            UiElementMetadata.Config.SplitterAction(),
                                            UiElementMetadata.Config.CreateAndCloseAction<Advertisement>(),
                                            UiElementMetadata.Config.UpdateAndCloseAction<Advertisement>(),
                                            UiElementMetadata.Config.SplitterAction(),
                                            UiElementMetadata.Config.RefreshAction<Advertisement>(),
                                            UiElementMetadata.Config.SplitterAction(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Preview")
                                                             .Title.Resource(() => ErmConfigLocalization.ControlPreviewAdvertisement)
                                                             .ControlType(ControlType.TextImageButton)
                                                             .LockOnNew()
                                                             .Handler.Name("scope.Preview")
                                                             .Icon.Path("PreviewAd.png"),
                                            UiElementMetadata.Config.SplitterAction(),
                                            UiElementMetadata.Config.CloseAction()),

                        #endregion

                        #region Bill
                        CardMetadata.For<Bill>()
                                    .ConfigBillToolbarWithPrinting(),

                        #endregion
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}