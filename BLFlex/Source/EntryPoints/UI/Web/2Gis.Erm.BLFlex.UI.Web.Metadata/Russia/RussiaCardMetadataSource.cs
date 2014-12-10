using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
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

                        CardMetadata.For<OrderProcessingRequest>()
                                    .MainAttribute<OrderProcessingRequest, IOrderProcessingRequestViewModel>(x => x.Title)
                                    .Actions.Attach(UiElementMetadata.Config.RefreshAction<OrderProcessingRequest>(),
                                                    UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config
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
                                                                                                                .Title.Resource(() =>
                                                                                                                                ErmConfigLocalization
                                                                                                                                    .ControlCancelOrderProcessingRequest)
                                                                                                                .LockOnNew()
                                                                                                                .Handler.Name("scope.CancelOrderProcessingRequest")
                                                                                                                .ControlType(ControlType.TextButton)
                                                                                                                .AccessWithPrivelege<OrderProcessingRequest>(EntityAccessTypes.Update)
                                                                                                                .Operation.NonCoupled<CancelOrderProcessingRequestIdentity>()),
                                                    UiElementMetadata.Config.CloseAction()),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}