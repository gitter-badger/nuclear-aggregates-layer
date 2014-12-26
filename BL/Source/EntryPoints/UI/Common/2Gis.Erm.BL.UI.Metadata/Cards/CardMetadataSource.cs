using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
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
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccounts),

                        CardMetadata.For<AccountDetail>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccountDetails),

                        CardMetadata.For<AdditionalFirmService>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.AdditionalFirmServices),

                        CardMetadata.For<AdsTemplatesAdsElementTemplate>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdsTemplatesAdsElementTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<Advertisement>()
                                    .MainAttribute<Advertisement, IAdvertisementViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisement)
                                    .ReadOnlyOn<IAdvertisementViewModel>(x => x.IsDummy)
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementElement>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElement)
                                    .ReadOnlyOn<IAdvertisementElementViewModel>(x => x.DisableEdit,
                                                                                x => x.CanUserChangeStatus,
                                                                                x => (x.NeedsValidation && x.Status != AdvertisementElementStatusValue.Draft))
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AdvertisementElementTemplate>()
                                    .MainAttribute<AdvertisementElementTemplate, IAdvertisementElementTemplateViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElementTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementTemplate>()
                                    .MainAttribute<AdvertisementTemplate, IAdvertisementTemplateViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementTemplate)
                                    .ReadOnlyOn<IAdvertisementTemplateViewModel>(x => x.IsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<Appointment>()
                                    .MainAttribute<Appointment, IAppointmentViewModel>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAppointment)
                                    .ReadOnlyOn<IAppointmentViewModel>(x => x.Status == ActivityStatus.Canceled,
                                                                       x => x.Status == ActivityStatus.Completed)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AssociatedPosition>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPosition)
                                    .ReadOnlyOn<IAssociatedPositionViewModel>(x => x.PriceIsDeleted,
                                                                              x => x.PriceIsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<AssociatedPositionsGroup>()
                                    .MainAttribute<AssociatedPositionsGroup, IAssociatedPositionsGroupViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPositionsGroup)
                                    .ReadOnlyOn<IAssociatedPositionsGroupViewModel>(x => x.PriceIsDeleted,
                                                                                    x => x.PriceIsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<Bargain>()
                                    .MainAttribute<Bargain, IBargainViewModel>(x => x.Number)
                                    .EntityLocalization(() => MetadataResources.Bargain)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<BargainFile>()
                                    .MainAttribute<BargainFile, IBargainFileViewModel>(x => x.FileName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainFile)
                                    .WithAdminTab(),

                        CardMetadata.For<BargainType>()
                                    .MainAttribute<BargainType, IBargainTypeViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainType)
                                    .WithAdminTab(),

                        CardMetadata.For<Bill>()
                                    .MainAttribute<Bill, IBillViewModel>(x => x.BillNumber)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBill)
                                    .WithAdminTab(),

                        CardMetadata.For<BranchOffice>()
                                    .MainAttribute<BranchOffice, IBranchOfficeViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOffices)
                                    .WithAdminTab(),

                        CardMetadata.For<BranchOfficeOrganizationUnit>()
                                    .MainAttribute<BranchOfficeOrganizationUnit, IBranchOfficeOrganizationUnitViewModel>(x => x.ShortLegalName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOfficeOrganizationUnit)
                                    .WithAdminTab(),

                        CardMetadata.For<Category>()
                                    .MainAttribute<Category, ICategoryViewModel>(x => x.Name)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategories)
                                    .WithAdminTab(),

                        CardMetadata.For<CategoryGroup>()
                                    .MainAttribute<CategoryGroup, ICategoryGroupViewModel>(x => x.CategoryGroupName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategoryGroups)
                                    .WithAdminTab(),

                        CardMetadata.For<Client>()
                                    .MainAttribute<Client, IClientViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnClient)
                                    .WithComments(),

                        CardMetadata.For<Contact>()
                                    .MainAttribute<Contact, IContactViewModel>(x => x.FullName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnContact)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<ContributionType>()
                                    .MainAttribute<ContributionType, IContributionTypeViewModel>(x => x.Name)
                                    .EntityLocalization(() => MetadataResources.ContributionType)
                                    .WithAdminTab(),

                        CardMetadata.For<Country>()
                                    .MainAttribute<Country, ICountryViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCountries)
                                    .WithAdminTab(),

                        CardMetadata.For<Currency>()
                                    .MainAttribute<Currency, ICurrencyViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCurrencies)
                                    .WithAdminTab(),

                        CardMetadata.For<CurrencyRate>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCurrencyRates)
                                    .WithAdminTab(),

                        CardMetadata.For<Deal>()
                                    .MainAttribute<Deal, IDealViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnDeal)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<DenialReason>()
                                    .MainAttribute<DenialReason, IDenialReasonViewModel>(x => x.Name)
                                    .EntityLocalization(() => MetadataResources.EnDenialReason)
                                    .WithAdminTab(),

                        CardMetadata.For<DeniedPosition>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnDeniedPosition)
                                    .ReadOnlyOn<IDeniedPositionViewModel>(x => x.IsPricePublished)
                                    .WithAdminTab(),

                        CardMetadata.For<Department>()
                                    .MainAttribute<Department, IDepartmentViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnDepartment)
                                    .WithAdminTab(),

                        CardMetadata.For<Firm>()
                                    .MainAttribute<Firm, IFirmViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirms)
                                    .WithComments(),

                        CardMetadata.For<FirmAddress>()
                                    .MainAttribute<FirmAddress, IFirmAddressViewModel>(x => x.Address)
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirmAddresses)
                                    .WithAdminTab(),

                        CardMetadata.For<FirmContact>()
                                    .MainAttribute<FirmContact, IFirmContactViewModel>(x => x.Contact)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirmContacts)
                                    .WithAdminTab(),

                        CardMetadata.For<LegalPerson>()
                                    .MainAttribute<LegalPerson, ILegalPersonViewModel>(x => x.LegalName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLegalPersons)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<LegalPersonProfile>()
                                    .MainAttribute<LegalPersonProfile, ILegalPersonProfileViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLegalPersonProfile)
                                    .WithAdminTab(),

                        CardMetadata.For<Letter>()
                                    .MainAttribute<Letter, ILetterViewModel>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLetter)
                                    .ReadOnlyOn<ILetterViewModel>(x => x.Status == ActivityStatus.Canceled,
                                                                  x => x.Status == ActivityStatus.Completed)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<Limit>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLimit)
                                    .ReadOnlyOn<ILimitViewModel>(x => x.Status != LimitStatus.Opened)
                                    .WithComments(),

                        CardMetadata.For<LocalMessage>()
                                    .MainAttribute(x => x.Id)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLocalMessage),

                        CardMetadata.For<Lock>()
                                    .MainAttribute(x => x.Id)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLocks)
                                    .WithAdminTab(),

                        CardMetadata.For<LockDetail>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLockDetails)
                                    .WithAdminTab(),

                        CardMetadata.For<Note>()
                                    .MainAttribute<Note, INoteViewModel>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnNote)
                                    .WithAdminTab(),

                        CardMetadata.For<Operation>()
                                    .MainAttribute<Operation, IOperationViewModel>(x => x.Type)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOperation)
                                    .WithAdminTab(),

                        CardMetadata.For<OperationType>()
                                    .MainAttribute<OperationType, IOperationTypeViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOperationTypes)
                                    .WithAdminTab(),

                        CardMetadata.For<Order>()
                                    .MainAttribute<Order, IOrderViewModel>(x => x.OrderNumber)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrders)
                                    .ReadOnlyOn<IOrderViewModel>(x => x.WorkflowStepId != (int)OrderState.OnRegistration)
                                    .WithComments(),

                        CardMetadata.For<OrderFile>()
                                    .MainAttribute<OrderFile, IOrderFileViewModel>(x => x.FileName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrderFile)
                                    .ReadOnlyOn<IOrderFileViewModel>(x => x.UserDoesntHaveRightsToEditOrder)
                                    .WithAdminTab(),

                        CardMetadata.For<OrderPosition>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrderPositions)
                                    .WithAdminTab(),

                        CardMetadata.For<OrganizationUnit>()
                                    .MainAttribute<OrganizationUnit, IOrganizationUnitViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrganizationUnits)
                                    .WithAdminTab(),

                        CardMetadata.For<Phonecall>()
                                    .MainAttribute<Phonecall, IPhonecallViewModel>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPhonecall)
                                    .ReadOnlyOn<IPhonecallViewModel>(x => x.Status == ActivityStatus.Canceled,
                                                                     x => x.Status == ActivityStatus.Completed)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<Platform.Model.Entities.Erm.Platform>()
                                    .MainAttribute<Platform.Model.Entities.Erm.Platform, IPlatformViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPlatform)
                                    .WithAdminTab(),

                        CardMetadata.For<Position>()
                                    .MainAttribute<Position, IPositionViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositions)
                                    .WithAdminTab(),

                        CardMetadata.For<PositionCategory>()
                                    .MainAttribute<PositionCategory, IPositionCategoryViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositionCategory)
                                    .WithAdminTab(),

                        CardMetadata.For<PositionChildren>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositionChildren)
                                    .WithAdminTab(),

                        CardMetadata.For<Price>()
                                    .MainAttribute<Price, IPriceViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPrices)
                                    .ReadOnlyOn<IPriceViewModel>(x => x.IsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<PricePosition>()
                                    .MainAttribute<PricePosition, IPricePositionViewModel>(x => x.Position.Value)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPricePositions)
                                    .WithAdminTab(),

                        CardMetadata.For<PrintFormTemplate>()
                                    .MainAttribute<PrintFormTemplate, IPrintFormTemplateViewModel>(x => x.FileName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPrintFormTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<Project>()
                                    .MainAttribute<Project, IProjectViewModel>(x => x.DisplayName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnProjects)
                                    .WithAdminTab(),

                        CardMetadata.For<ReleaseInfo>()
                                    .MainAttribute(x => x.Id)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnReleaseInfo)
                                    .WithAdminTab(),

                        CardMetadata.For<Role>()
                                    .MainAttribute<Role, IRoleViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnRole)
                                    .WithAdminTab(),

                        CardMetadata.For<RolePrivilege>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnRolePrivilege)
                                    .WithAdminTab(),

                        CardMetadata.For<Task>()
                                    .MainAttribute<Task, ITaskViewModel>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnTask)
                                    .ReadOnlyOn<ITaskViewModel>(x => x.Status == ActivityStatus.Canceled,
                                                                x => x.Status == ActivityStatus.Completed)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<Territory>()
                                    .MainAttribute<Territory, ITerritoryViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnTerritory)
                                    .WithAdminTab(),

                        CardMetadata.For<Theme>()
                                    .MainAttribute<Theme, IThemeViewModel>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnTheme)
                                    .WithAdminTab(),

                        CardMetadata.For<ThemeTemplate>()
                                    .MainAttribute<ThemeTemplate, IThemeTemplateViewModel>(x => x.TemplateCode)
                                    .EntityLocalization(() => ErmConfigLocalization.EnThemeTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<User>()
                                    .MainAttribute<User, IUserViewModel>(x => x.DisplayName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnUser)
                                    .WithAdminTab(),

                        CardMetadata.For<UserProfile>()
                                    .MainAttribute(x => x.Id)
                                    .EntityLocalization(() => ErmConfigLocalization.EnUserProfile)
                                    .WithAdminTab(),

                        CardMetadata.For<WithdrawalInfo>()
                                    .MainAttribute(x => x.Id)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnWithdrawalInfo)
                                    .WithAdminTab()
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}