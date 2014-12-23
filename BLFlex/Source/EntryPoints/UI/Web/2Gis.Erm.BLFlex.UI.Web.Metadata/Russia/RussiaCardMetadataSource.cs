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
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

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
                                    .Actions.Attach(UIElementMetadata.Config.RefreshAction<OrderProcessingRequest>(),
                                                    UIElementMetadata.Config.AdditionalActions
                                                        (UIElementMetadata.Config
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
                                                         UIElementMetadata.Config
                                                                          .Name.Static("CancelOrderProcessingRequest")
                                                                          .Title.Resource(() => ErmConfigLocalization.ControlCancelOrderProcessingRequest)
                                                                          .LockOnNew()
                                                                          .Handler.Name("scope.CancelOrderProcessingRequest")
                                                                          .ControlType(ControlType.TextButton)
                                                                          .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                                                          .Operation.NonCoupled<CancelOrderProcessingRequestIdentity>()),
                                                    UIElementMetadata.Config.CloseAction()),

                        #endregion

                        #region LegalPerson
                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UIElementMetadata.Config.CommonLegalPersonAdditionalActions()
                                                                                                            .With(UIElementMetadata.Config.MergeLegalPersonsAction())),

                        #endregion

                        #region Order
                        CardMetadata.For<Order>()
                                    .ConfigOrderToolbarWithSpecificPrintActions(UIElementMetadata.Config.RussianOrderPrintActions())
                                    .WithRelatedItems(UIElementMetadata.Config.CommonOrderRelatedActions()
                                                                         .With(UIElementMetadata.Config.Name.Static("OrderProcessingRequests")
                                                                                                .Title.Resource(() => ErmConfigLocalization.CrdRelOrderProcessingRequests)
                                                                                                .LockOnNew()
                                                                                                .Handler.ShowGridByConvention(EntityName.OrderProcessingRequest)
                                                                                                .FilterToParent())),

                        #endregion

                        #region Bargain
                        CardMetadata.For<Bargain>()
                                    .ConfigBargainToolbarWithSpecificPrintActions(UIElementMetadata.Config.PrintBargainAction(),
                                                                                  UIElementMetadata.Config.PrintNewSalesModelBargainAction(),
                                                                                  UIElementMetadata.Config.PrintBargainProlongationAgreementAction()),

                        #endregion

                        #region Advertisement
                        CardMetadata.For<Advertisement>()
                                    .Actions
                                    .Attach(UIElementMetadata.Config.CreateAction<Advertisement>(),
                                            UIElementMetadata.Config.UpdateAction<Advertisement>(),
                                            UIElementMetadata.Config.SplitterAction(),
                                            UIElementMetadata.Config.CreateAndCloseAction<Advertisement>(),
                                            UIElementMetadata.Config.UpdateAndCloseAction<Advertisement>(),
                                            UIElementMetadata.Config.SplitterAction(),
                                            UIElementMetadata.Config.RefreshAction<Advertisement>(),
                                            UIElementMetadata.Config.SplitterAction(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Preview")
                                                             .Title.Resource(() => ErmConfigLocalization.ControlPreviewAdvertisement)
                                                             .ControlType(ControlType.TextImageButton)
                                                             .LockOnNew()
                                                             .Handler.Name("scope.Preview")
                                                             .Icon.Path("PreviewAd.png"),
                                            UIElementMetadata.Config.SplitterAction(),
                                            UIElementMetadata.Config.CloseAction()),

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