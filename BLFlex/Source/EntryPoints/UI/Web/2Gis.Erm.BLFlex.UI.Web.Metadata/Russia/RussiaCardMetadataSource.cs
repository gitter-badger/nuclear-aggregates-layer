using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Entities;

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
                                    .MVVM.Bind<DealViewModel>("~/Views/CreateOrUpdate/Russia/Deal.cshtml"),

                        CardMetadata.For<OrderPosition>()
                                    .MVVM.Bind<OrderPositionViewModel>("~/Views/CreateOrUpdate/Russia/OrderPosition.cshtml"),

                        CardMetadata.For<AdvertisementElementStatus>()
                                    .WithDefaultIcon(),

                        CardMetadata.For<OrderProcessingRequest>()
                                    .WithDefaultIcon()
                                    .Actions.Attach(ToolbarElements.Refresh<OrderProcessingRequest>(),
                                                    ToolbarElements.Additional(ToolbarElementsFlex.OrderProcessingRequests
                                                                                                  .CreateOrder()
                                                                                                  .DisableOn<IOrderProcessingRequestStateAspect>(x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Cancelled,
                                                                                                                                               x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Completed),
                                                                               ToolbarElementsFlex.OrderProcessingRequests
                                                                                                  .Cancel()
                                                                                                  .DisableOn<IOrderProcessingRequestStateAspect>(x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Cancelled,
                                                                                                                                               x =>
                                                                                                                                               x.State ==
                                                                                                                                               OrderProcessingRequestState.Completed)),
                                                    ToolbarElements.Close()),

                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UIElementMetadata.Config.CommonLegalPersonAdditionalActions()
                                                                                                            .With(ToolbarElementsFlex.LegalPersons.Russia.Merge())),

                        CardMetadata.For<Order>()
                                    .RussiConfigOrderToolbarWithSpecificPrintActions(UIElementMetadata.Config.RussianOrderPrintActions())
                                    .WithRelatedItems(UIElementMetadata.Config.CommonOrderRelatedActions()
                                                                       .With(RelatedItem.EntityGrid(EntityType.Instance.OrderProcessingRequest(),
                                                                                                     () => ErmConfigLocalization.CrdRelOrderProcessingRequests))),

                        CardMetadata.For<Bargain>()
                                    .ConfigBargainToolbarWithSpecificPrintActions(ToolbarElementsFlex.Bargains.PrintBargain(),
                                                                                  ToolbarElementsFlex.Bargains.Russia.PrintNewSalesModelBargainAction(),
                                                                                  ToolbarElementsFlex.Bargains.PrintBargainProlongation()),

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

                        CardMetadata.For<Bill>()
                                    .ConfigBillToolbarWithPrinting(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}