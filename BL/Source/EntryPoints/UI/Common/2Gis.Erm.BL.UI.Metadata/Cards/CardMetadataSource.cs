using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
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
                                    .WithDefaultIcon(),

                        CardMetadata.For<AdditionalFirmService>()
                                    .EntityLocalization(() => ErmConfigLocalization.AdditionalFirmServices)
                                    .WithDefaultIcon(),

                        CardMetadata.For<AdsTemplatesAdsElementTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdsTemplatesAdsElementTemplate)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Advertisement>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisement)
                                    .Icon.Path("en_ico_16_Advertisement.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementElement>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElement)
                                    .WithDefaultIcon()
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AdvertisementElementTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElementTemplate)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementTemplate)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Appointment>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAppointment)
                                    .Icon.Path("en_ico_lrg_Appointment.gif")
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AssociatedPosition>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPosition)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<AssociatedPositionsGroup>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPositionsGroup)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Bargain>()
                                    .EntityLocalization(() => MetadataResources.Bargain)
                                    .Icon.Path("en_ico_lrg_Bargain.gif")
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<BargainFile>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainFile)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<BargainType>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainType)
                                    .Icon.Path("en_ico_lrg_BargainType.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Bill>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBill)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<BranchOffice>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOffices)
                                    .Icon.Path("en_ico_lrg_BranchOffice.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<BranchOfficeOrganizationUnit>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOfficeOrganizationUnit)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Category>()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategories)
                                    .Icon.Path("en_ico_lrg_Category.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<CategoryGroup>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategoryGroups)
                                    .Icon.Path("en_ico_lrg_Category.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Client>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnClient)
                                    .Icon.Path("en_ico_16_Client.gif")
                                    .WithComments(),

                        CardMetadata.For<Contact>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnContact)
                                    .Icon.Path("en_ico_lrg_Contact.gif")
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<ContributionType>()
                                    .EntityLocalization(() => MetadataResources.ContributionType)
                                    .Icon.Path("en_ico_lrg_ContributionType.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Country>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCountries)
                                    .Icon.Path("en_ico_lrg_Country.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Currency>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCurrencies)
                                    .Icon.Path("en_ico_lrg_Currency.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<CurrencyRate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCurrencyRates)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Deal>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnDeal)
                                    .Icon.Path("en_ico_lrg_Deal.gif")
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<DenialReason>()
                                    .EntityLocalization(() => MetadataResources.EnDenialReason)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<DeniedPosition>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnDeniedPosition)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Department>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnDepartment)
                                    .Icon.Path("en_ico_lrg_Department.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Firm>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirms)
                                    .Icon.Path("en_ico_16_Firm.gif")
                                    .WithComments(),

                        CardMetadata.For<FirmAddress>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirmAddresses)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<FirmContact>()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirmContacts)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<LegalPerson>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLegalPersons)
                                    .WithDefaultIcon()
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<LegalPersonProfile>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLegalPersonProfile)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Letter>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLetter)
                                    .Icon.Path("en_ico_lrg_Letter.gif")
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<Limit>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLimit)
                                    .WithDefaultIcon()
                                    .WithComments(),

                        CardMetadata.For<LocalMessage>()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLocalMessage)
                                    .Icon.Path("Default.gif"),

                        CardMetadata.For<Lock>()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLocks)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<LockDetail>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLockDetails)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Note>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnNote)
                                    .Icon.Path("en_ico_lrg_Note.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Operation>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOperation)
                                    .Icon.Path("en_ico_lrg_LocalMessage.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<OperationType>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOperationTypes)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Order>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrders)
                                    .Icon.Path("en_ico_16_Order.gif")
                                    .WithComments(),

                        CardMetadata.For<OrderFile>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrderFile)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<OrderPosition>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrderPositions)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<OrganizationUnit>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrganizationUnits)
                                    .Icon.Path("en_ico_lrg_OrganizationUnit.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Phonecall>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPhonecall)
                                    .Icon.Path("en_ico_lrg_Phonecall.gif")
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<Platform.Model.Entities.Erm.Platform>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPlatform)
                                    .Icon.Path("en_ico_lrg_Platform.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Position>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositions)
                                    .Icon.Path("en_ico_lrg_Position.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<PositionCategory>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositionCategory)
                                    .Icon.Path("en_ico_lrg_PositionCategory.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<PositionChildren>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositionChildren)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Price>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPrices)
                                    .Icon.Path("en_ico_lrg_Price.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<PricePosition>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPricePositions)
                                    .Icon.Path("en_ico_lrg_PricePosition.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<PrintFormTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPrintFormTemplate)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Project>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnProjects)
                                    .Icon.Path("en_ico_16_Default.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<ReleaseInfo>()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnReleaseInfo)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<Role>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnRole)
                                    .Icon.Path("en_ico_lrg_UserGroup.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<RolePrivilege>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnRolePrivilege)
                                    .Icon.Path("en_ico_lrg_UserAccountGroup.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Task>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnTask)
                                    .Icon.Path("en_ico_lrg_Task.gif")
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<Territory>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnTerritory)
                                    .Icon.Path("en_ico_lrg_Territory.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<Theme>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnTheme)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<ThemeTemplate>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnThemeTemplate)
                                    .WithDefaultIcon()
                                    .WithAdminTab(),

                        CardMetadata.For<User>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnUser)
                                    .Icon.Path("en_ico_lrg_UserAccount.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<UserProfile>()
                                    .EntityLocalization(() => ErmConfigLocalization.EnUserProfile)
                                    .Icon.Path("en_ico_lrg_UserAccount.gif")
                                    .WithAdminTab(),

                        CardMetadata.For<WithdrawalInfo>()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnWithdrawalInfo)
                                    .WithDefaultIcon()
                                    .WithAdminTab()
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}