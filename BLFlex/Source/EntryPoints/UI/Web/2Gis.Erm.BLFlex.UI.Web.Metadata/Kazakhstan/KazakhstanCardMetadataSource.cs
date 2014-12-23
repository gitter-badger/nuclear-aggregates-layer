using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Kazakhstan
{
    public sealed class KazakhstanCardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>, IKazakhstanAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public KazakhstanCardMetadataSource()
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
                                    .MVVM.Bind<IDealViewModel>("~/Views/CreateOrUpdate/Kazakhstan/Deal.cshtml"),

                        CardMetadata.For<OrderPosition>()
                                    .MVVM.Bind<IOrderPositionViewModel>("~/Views/CreateOrUpdate/Kazakhstan/OrderPosition.cshtml"),

                        CardMetadata.For<Advertisement>()
                                    .CommonCardToolbar(),

                        CardMetadata.For<Bargain>()
                                    .ConfigBargainToolbarWithSpecificPrintActions(UIElementMetadata.Config.PrintBargainAction(),
                                                                                  UIElementMetadata.Config.PrintBargainProlongationAgreementAction()),

                        CardMetadata.For<Bill>()
                                    .ConfigBillToolbarWithPrinting(),

                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UIElementMetadata.Config.CommonLegalPersonAdditionalActions()),

                        CardMetadata.For<Order>()
                                    .WithRelatedItems(UIElementMetadata.Config.CommonOrderRelatedActions())
                                    .ConfigOrderToolbarWithSpecificPrintActions(UIElementMetadata.Config.KazakhstanOrderPrintActions()),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}