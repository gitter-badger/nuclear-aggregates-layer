using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

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
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccounts),

                        CardMetadata.For<AccountDetail>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAccountDetails),

                        CardMetadata.For<AdsTemplatesAdsElementTemplate>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdsTemplatesAdsElementTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<Advertisement>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisement)
                                    .ReadOnlyOn<IDummyAdvertisementAspect>(x => x.IsDummy)
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementElement>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElement)
                                    .ReadOnlyOn(Condition.On<ISetReadOnlyAspect>(x => x.SetReadonly) |
                                                Condition.On<IAdvertisementElementVerificationAspect>(x => x.CanUserChangeStatus) |
                                                Condition.On<IAdvertisementElementVerificationAspect>(x => x.NeedsValidation && x.Status != AdvertisementElementStatusValue.Draft))
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AdvertisementElementTemplate>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementElementTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<AdvertisementTemplate>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAdvertisementTemplate)
                                    .ReadOnlyOn<IPublishableAspect>(x => x.IsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<Appointment>()
                                    .MainAttribute<ITitleAspect>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAppointment)
                                    .ReadOnlyOn<IActivityStateAspect>(x => x.Status == ActivityStatus.Canceled,
                                                                      x => x.Status == ActivityStatus.Completed)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<AssociatedPosition>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPosition)
                                    .ReadOnlyOn(Condition.On<IDeletablePriceAspect>(x => x.PriceIsDeleted) |
                                                Condition.On<IPublishablePriceAspect>(x => x.PriceIsPublished))
                                    .WithAdminTab(),

                        CardMetadata.For<AssociatedPositionsGroup>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnAssociatedPositionsGroup)
                                    .ReadOnlyOn(Condition.On<IDeletablePriceAspect>(x => x.PriceIsDeleted) |
                                                Condition.On<IPublishablePriceAspect>(x => x.PriceIsPublished))
                                    .WithAdminTab(),

                        CardMetadata.For<Bargain>()
                                    .MainAttribute<INumberAspect>(x => x.Number)
                                    .EntityLocalization(() => MetadataResources.Bargain)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<BargainFile>()
                                    .MainAttribute<IFileNameAspect>(x => x.FileName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainFile)
                                    .WithAdminTab(),

                        CardMetadata.For<BargainType>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBargainType)
                                    .WithAdminTab(),

                        CardMetadata.For<Bill>()
                                    .MainAttribute<INumberAspect>(x => x.Number)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBill)
                                    .WithAdminTab(),

                        CardMetadata.For<BranchOffice>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOffices)
                                    .WithAdminTab(),

                        CardMetadata.For<BranchOfficeOrganizationUnit>()
                                    .MainAttribute<IShortLegalNameAspect>(x => x.ShortLegalName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnBranchOfficeOrganizationUnit)
                                    .WithAdminTab(),

                        CardMetadata.For<Category>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategories)
                                    .WithAdminTab(),

                        CardMetadata.For<CategoryGroup>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCategoryGroups)
                                    .WithAdminTab(),

                        CardMetadata.For<Client>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnClient)
                                    .WithComments(),

                        CardMetadata.For<Contact>()
                                    .MainAttribute<IFullNameAspect>(x => x.FullName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnContact)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<ContributionType>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => MetadataResources.ContributionType)
                                    .WithAdminTab(),

                        CardMetadata.For<Country>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCountries)
                                    .WithAdminTab(),

                        CardMetadata.For<Currency>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnCurrencies)
                                    .WithAdminTab(),

                        CardMetadata.For<CurrencyRate>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnCurrencyRates)
                                    .WithAdminTab(),

                        CardMetadata.For<Deal>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnDeal)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<DenialReason>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => MetadataResources.EnDenialReason)
                                    .WithAdminTab(),

                        CardMetadata.For<DeniedPosition>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnDeniedPosition)
                                    .ReadOnlyOn<IPublishablePriceAspect>(x => x.PriceIsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<Department>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnDepartment)
                                    .WithAdminTab(),

                        CardMetadata.For<Firm>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirms)
                                    .WithComments(),

                        CardMetadata.For<FirmAddress>()
                                    .MainAttribute<IAddressAspect>(x => x.Address)
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirmAddresses)
                                    .WithAdminTab(),

                        CardMetadata.For<FirmContact>()
                                    .MainAttribute<IContactAspect>(x => x.Contact)
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnFirmContacts)
                                    .WithAdminTab(),

                        CardMetadata.For<LegalPerson>()
                                    .MainAttribute<ILegalNameAspect>(x => x.LegalName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLegalPersons)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<LegalPersonProfile>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLegalPersonProfile)
                                    .WithAdminTab(),

                        CardMetadata.For<Letter>()
                                    .MainAttribute<ITitleAspect>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnLetter)
                                    .ReadOnlyOn<IActivityStateAspect>(x => x.Status == ActivityStatus.Canceled,
                                                                      x => x.Status == ActivityStatus.Completed)
                                    .WithAdminTab()
                                    .WithComments(),

                        CardMetadata.For<Limit>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLimit)
                                    .ReadOnlyOn<ILimitStateAspect>(x => x.Status != LimitStatus.Opened)
                                    .WithComments(),

                        CardMetadata.For<LocalMessage>()
                                    .WithDefaultMainAttribute()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLocalMessage),

                        CardMetadata.For<Lock>()
                                    .WithDefaultMainAttribute()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLocks)
                                    .WithAdminTab(),

                        CardMetadata.For<LockDetail>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnLockDetails)
                                    .WithAdminTab(),

                        CardMetadata.For<Note>()
                                    .MainAttribute<ITitleAspect>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnNote)
                                    .WithAdminTab(),

                        CardMetadata.For<Operation>()
                                    .MainAttribute<IBusinessOperationTypeAspect>(x => x.Type)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOperation)
                                    .WithAdminTab(),

                        CardMetadata.For<OperationType>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOperationTypes)
                                    .WithAdminTab(),

                        CardMetadata.For<Order>()
                                    .MainAttribute<INumberAspect>(x => x.Number)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrders)
                                    .ReadOnlyOn<IOrderWorkflowAspect>(x => x.WorkflowStepId != OrderState.OnRegistration)
                                    .WithComments(),

                        CardMetadata.For<OrderFile>()
                                    .MainAttribute<IFileNameAspect>(x => x.FileName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrderFile)
                                    .ReadOnlyOn<ISetReadOnlyAspect>(x => x.SetReadonly)
                                    .WithAdminTab(),

                        CardMetadata.For<OrderPosition>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrderPositions)
                                    .WithAdminTab(),

                        CardMetadata.For<OrganizationUnit>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnOrganizationUnits)
                                    .WithAdminTab(),

                        CardMetadata.For<Phonecall>()
                                    .MainAttribute<ITitleAspect>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPhonecall)
                                    .ReadOnlyOn<IActivityStateAspect>(x => x.Status == ActivityStatus.Canceled,
                                                                      x => x.Status == ActivityStatus.Completed)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<Platform.Model.Entities.Erm.Platform>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPlatform)
                                    .WithAdminTab(),

                        CardMetadata.For<Position>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositions)
                                    .WithAdminTab(),

                        CardMetadata.For<PositionCategory>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositionCategory)
                                    .WithAdminTab(),

                        CardMetadata.For<PositionChildren>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnPositionChildren)
                                    .WithAdminTab(),

                        CardMetadata.For<Price>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPrices)
                                    .ReadOnlyOn<IPublishableAspect>(x => x.IsPublished)
                                    .WithAdminTab(),

                        CardMetadata.For<PricePosition>()
                                    .MainAttribute<IPositionAspect>(x => x.PositionName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPricePositions)
                                    .WithAdminTab(),

                        CardMetadata.For<PrintFormTemplate>()
                                    .MainAttribute<IFileNameAspect>(x => x.FileName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnPrintFormTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<Project>()
                                    .MainAttribute<IDisplayNameAspect>(x => x.DisplayName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnProjects)
                                    .WithAdminTab(),

                        CardMetadata.For<ReleaseInfo>()
                                    .WithDefaultMainAttribute()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnReleaseInfo)
                                    .WithAdminTab(),

                        CardMetadata.For<Role>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnRole)
                                    .WithAdminTab(),

                        CardMetadata.For<RolePrivilege>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnRolePrivilege)
                                    .WithAdminTab(),

                        CardMetadata.For<Task>()
                                    .MainAttribute<ITitleAspect>(x => x.Title)
                                    .EntityLocalization(() => ErmConfigLocalization.EnTask)
                                    .ReadOnlyOn<IActivityStateAspect>(x => x.Status == ActivityStatus.Canceled,
                                                                      x => x.Status == ActivityStatus.Completed)
                                    .WithComments()
                                    .WithAdminTab(),

                        CardMetadata.For<Territory>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnTerritory)
                                    .WithAdminTab(),

                        CardMetadata.For<Theme>()
                                    .MainAttribute<INameAspect>(x => x.Name)
                                    .EntityLocalization(() => ErmConfigLocalization.EnTheme)
                                    .WithAdminTab(),

                        CardMetadata.For<ThemeTemplate>()
                                    .MainAttribute<IThemeTemplateCodeAspect>(x => x.TemplateCode)
                                    .EntityLocalization(() => ErmConfigLocalization.EnThemeTemplate)
                                    .WithAdminTab(),

                        CardMetadata.For<User>()
                                    .MainAttribute<IDisplayNameAspect>(x => x.DisplayName)
                                    .EntityLocalization(() => ErmConfigLocalization.EnUser)
                                    .WithAdminTab(),

                        CardMetadata.For<UserProfile>()
                                    .WithDefaultMainAttribute()
                                    .EntityLocalization(() => ErmConfigLocalization.EnUserProfile)
                                    .WithAdminTab(),

                        CardMetadata.For<WithdrawalInfo>()
                                    .WithDefaultMainAttribute()
                                    .ReadOnly()
                                    .EntityLocalization(() => ErmConfigLocalization.EnWithdrawalInfo)
                                    .WithAdminTab()
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}