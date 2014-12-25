﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.BLFlex.UI.Metadata.ViewModels.Contracts.Chile;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Chile
{
    public sealed class ChileCardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>, IChileAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ChileCardMetadataSource()
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
                                    .MVVM.Bind<IDealViewModel>("~/Views/CreateOrUpdate/Chile/Deal.cshtml"),

                        CardMetadata.For<OrderPosition>()
                                    .MVVM.Bind<IOrderPositionViewModel>("~/Views/CreateOrUpdate/Chile/OrderPosition.cshtml"),

                        CardMetadata.For<Bank>()
                                    .MainAttribute<Bank, IBankViewModel>(x => x.Name)
                                    .CommonCardToolbar(),

                        CardMetadata.For<Advertisement>()
                                    .CommonCardToolbar(),

                        CardMetadata.For<Bargain>()
                                    .ConfigBargainToolbarWithSpecificPrintActions(ToolbarElementsFlex.Bargains.PrintBargain()),

                        CardMetadata.For<Bill>()
                                    .ConfigBillToolbarWithPrinting(),

                        CardMetadata.For<LegalPerson>()
                                    .ConfigLegalPersonToolbarWithSpecificAdditionalActions(UIElementMetadata.Config.CommonLegalPersonAdditionalActions()),

                        CardMetadata.For<Order>()
                                    .WithRelatedItems(UIElementMetadata.Config.CommonOrderRelatedActions())
                                    .ConfigOrderToolbarWithSpecificPrintActions(UIElementMetadata.Config.ChileOrderPrintActions())
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}