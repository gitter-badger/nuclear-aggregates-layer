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
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdsTemplatesAdsElementTemplate)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<DenialReason>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => MetadataResources.EnDenialReason)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<AdvertisementTemplate>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementTemplate)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<AdvertisementElementTemplate>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElementTemplate)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<Advertisement>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisement)
                                    .Icon.Path("en_ico_16_Advertisement.gif"),

                        CardMetadata.For<AdvertisementElement>()
                                    .WithAdminTab()
                                    .WithComments()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElement)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<AdvertisementElementStatus>()
                                    .WithAdminTab()
                                    .WithComments()
                                    .EntityLocalization(() => EnumResources.EntityNameAdvertisementElementStatus)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<Appointment>()
                                    .WithAdminTab()
                                    .WithComments()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAppointment)
                                    .Icon.Path("en_ico_16_Appointment.gif"),

                        CardMetadata.For<AssociatedPosition>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPosition)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<AssociatedPositionsGroup>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPositionsGroup)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<Bargain>()
                                    .WithComments()
                                    .WithAdminTab()
                                    .EntityLocalization(() => MetadataResources.Bargain)
                                    .Icon.Path("en_ico_lrg_Bargain.gif"),

                        CardMetadata.For<BargainFile>()
                                    .WithComments()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainFile)
                                    .Icon.Path("en_ico_lrg_Bargain.gif"),

                        CardMetadata.For<BargainType>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainType)
                                    .Icon.Path("en_ico_lrg_BargainType.gif"),

                        CardMetadata.For<Bill>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBill)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<BranchOffice>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOffices)
                                    .Icon.Path("en_ico_lrg_BranchOffice.gif"),

                        CardMetadata.For<BranchOfficeOrganizationUnit>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOfficeOrganizationUnit)
                                    .Icon.Path("en_ico_lrg_Default.gif"),

                        CardMetadata.For<Category>()
                                    .WithAdminTab()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategories)
                                    .Icon.Path("en_ico_lrg_Category.gif"),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}