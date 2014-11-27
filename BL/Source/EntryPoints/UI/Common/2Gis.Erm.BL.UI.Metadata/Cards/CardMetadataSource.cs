using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
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
                                    .Icon.Path("en_ico_16_Account.gif"),

                        CardMetadata.For<AccountDetail>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccountDetails)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<AdditionalFirmService>()
                                    .EntityLocalization(() => ErmConfigLocalization.AdditionalFirmServices)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<AdsTemplatesAdsElementTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdsTemplatesAdsElementTemplate)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<DenialReason>()
                                    .EntityLocalization(() => MetadataResources.EnDenialReason)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementTemplate)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementElementTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElementTemplate)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Advertisement>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisement)
                                    .Icon.Path("en_ico_16_Advertisement.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementElement>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElement)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AdvertisementElementStatus>()
                                    .EntityLocalization(() => EnumResources.EntityNameAdvertisementElementStatus)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<Appointment>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAppointment)
                                    .Icon.Path("en_ico_16_Appointment.gif")
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AssociatedPosition>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPosition)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<AssociatedPositionsGroup>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPositionsGroup)
                                    .Icon.Path("en_ico_lrg_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Bargain>()
                                    .EntityLocalization(() => MetadataResources.Bargain)
                                    .Icon.Path("en_ico_lrg_Bargain.gif")
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<BargainFile>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainFile)
                                    .Icon.Path("en_ico_lrg_Bargain.gif")
                                    .WithComments()
                                    .WithAdminTab(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}