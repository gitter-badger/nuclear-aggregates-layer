using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

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
                                    .WithDefaultIcon(),

                        #region OrderProcessingRequest
                        CardMetadata.For<OrderProcessingRequest>()
                                    .WithDefaultIcon()
                                    .Actions.Attach(ToolbarElements.Refresh<OrderProcessingRequest>(),
                                                    ToolbarElements.Additional(ToolbarElementsFlex.OrderProcessingRequests.CreateOrder()
                                                                                                  .DisableOn<IOrderProcessingRequestViewModel>(
                                                                                                                                               x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Cancelled,
                                                                                                                                               x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Completed),
                                                                               ToolbarElementsFlex.OrderProcessingRequests.Cancel()
                                                                                                  .DisableOn<IOrderProcessingRequestViewModel>(
                                                                                                                                               x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Cancelled,
                                                                                                                                               x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Completed)),
                                                    ToolbarElements.Close()),

                        #endregion

                        #region LegalPerson
                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UIElementMetadata.Config.CommonLegalPersonAdditionalActions()
                                                                                                            .With(ToolbarElementsFlex.LegalPersons.Russia.Merge())),

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
                                    .ConfigBargainToolbarWithSpecificPrintActions(ToolbarElementsFlex.Bargains.PrintBargain(),
                                                                                  ToolbarElementsFlex.Bargains.Russia.PrintNewSalesModelBargainAction(),
                                                                                  ToolbarElementsFlex.Bargains.PrintBargainProlongation()),

                        #endregion

                        #region Advertisement
                        CardMetadata.For<Advertisement>()
                                    .Actions
                                    .Attach(ToolbarElements.Create<Advertisement>(),
                                            ToolbarElements.Update<Advertisement>(),
                                            ToolbarElements.Splitter(),
                                            ToolbarElements.CreateAndClose<Advertisement>(),
                                            ToolbarElements.UpdateAndClose<Advertisement>(),
                                            ToolbarElements.Splitter(),
                                            ToolbarElements.Refresh<Advertisement>(),
                                            ToolbarElements.Splitter(),
                                            ToolbarElementsFlex.Advertisements.Preview(),
                                            ToolbarElements.Splitter(),
                                            ToolbarElements.Close()),

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