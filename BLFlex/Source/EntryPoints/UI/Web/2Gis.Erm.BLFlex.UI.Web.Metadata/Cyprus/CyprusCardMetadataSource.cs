using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Cyprus
{
    public sealed class CyprusCardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>, ICyprusAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public CyprusCardMetadataSource()
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
                                    .MVVM.Bind<DealViewModel>("~/Views/CreateOrUpdate/Cyprus/Deal.cshtml"),

                        CardMetadata.For<OrderPosition>()
                                    .MVVM.Bind<OrderPositionViewModel>("~/Views/CreateOrUpdate/Cyprus/OrderPosition.cshtml"),

                        CardMetadata.For<Advertisement>()
                                    .CommonCardToolbar(),

                        CardMetadata.For<Bargain>()
                                    .ConfigBargainToolbarWithSpecificPrintActions(ToolbarElementsFlex.Bargains.PrintBargain()),

                        CardMetadata.For<Bill>()
                                    .CommonCardToolbar(),

                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UIElementMetadata.Config.CommonLegalPersonAdditionalActions()),

                        CardMetadata.For<Order>()
                                    .WithRelatedItems(UIElementMetadata.Config.CommonOrderRelatedActions())
                                    .MultiCultureConfigOrderToolbarWithSpecificPrintActions(UIElementMetadata.Config.CyprusOrderPrintActions()),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}