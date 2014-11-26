using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards
{
    public sealed class CardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public CardMetadataSource()
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
                        CardMetadata.For<Account>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccounts)
                                    .Icon.Path("en_ico_16_Account.gif")
                                    .MainAttribute(x => x.Id),

                        CardMetadata.For<AccountDetail>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccountDetails)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .MainAttribute(x => x.Id)
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}