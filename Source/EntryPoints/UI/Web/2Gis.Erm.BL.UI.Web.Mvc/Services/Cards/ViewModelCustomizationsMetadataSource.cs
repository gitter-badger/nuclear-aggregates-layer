using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Accounts;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Activities;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementTemplates;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositionGroups;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Clients;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Contacts;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Deals;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.DeniedPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersonProfiles;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersons;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LockDetails;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Locks;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderFiles;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderProcessingRequests;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Positions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.PricePositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Prices;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Territories;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Themes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationsMetadataSource : MetadataSourceBase<ViewModelCustomizationsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ViewModelCustomizationsMetadataSource()
        {
            _metadata = InitializeMetadataContainer();
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private static IReadOnlyDictionary<Uri, IMetadataElement> InitializeMetadataContainer()
        {
            IReadOnlyCollection<ViewModelCustomizationsMetadata> metadataContainer =
                new ViewModelCustomizationsMetadata[]
                    {
                        ViewModelCustomizationsMetadata.Config
                                                     .For<Account>()
                                                     .Use<AccountIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Advertisement>()
                                                     .Use<AdvertisementAccessCustomization>()
                                                     .Use<DummyAdvertisementCustomization>()
                                                     .Use<SelectedToWhiteListAdvertisementCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<AdvertisementElement>()
                                                     .Use<AdvertisementElementFasCommentCustomization>()
                                                     .Use<CheckIfAdvertisementElementReadOnly>()
                                                     .Use<ManageAdvertisementElementWorkflowButtonsCustomizations>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<AdvertisementTemplate>()
                                                     .Use<ManageAdvertisementTemplatePublicationButtonsCustomization>()
                                                     .Use<PublishedAdvertisementTemplateCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Appointment>()
                                                     .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<AssociatedPosition>()
                                                     .Use<AssociatedPositionsPriceIsDeletedCustomization>()
                                                     .Use<AssociatedPositionsPriceIsPublishedCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<AssociatedPositionsGroup>()
                                                     .UseOrdered<AssociatedPositionGroupIsDeletedCustomization>()
                                                     .UseOrdered<AssociatedPositionGroupsPriceIsDeletedCustomization>()
                                                     .UseOrdered<AssociatedPositionGroupsPriceIsPublishedCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Category>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Client>()
                                                     .Use<WarnLinkToAdvAgencyExistsVmCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Contact>()
                                                     .Use<BusinessModelAreaCustomization>()
                                                     .Use<ContactSalutationsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Deal>()
                                                     .Use<DisableReopenDealButtonCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<DeniedPosition>()
                                                     .UseOrdered<DeniedPositionsPriceIsPublishedCustomization>()
                                                     .UseOrdered<InactiveDeniedPositionsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Firm>()
                                                     .Use<ChangeTerritoryPrivilegeCustomization>()
                                                     .Use<FirmIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<FirmContact>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<LegalPerson>()
                                                     .UseOrdered<LegalPersonDoesntHaveAnyProfilesCustomization>()
                                                     .UseOrdered<LegalPersonIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<LegalPersonProfile>()
                                                     .Use<MainLegalPersonProfileCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Limit>()
                                                     .Use<CheckIfLimitRecalculationAvailableCustomization>()
                                                     .Use<CheckLimitPrivilegeCustomization>()
                                                     .Use<LockLimitByWorkflowCustomization>()
                                                     .Use<ManageLimitWorkflowButtonsCustomization>()
                                                     .Use<SetLimitInspectorNameCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<LocalMessage>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Lock>()
                                                     .Use<SetReadonlyCustomization>()
                                                     .Use<NewLockCustomization>()
                                                     .Use<LocalizeLockStatusCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<LockDetail>()
                                                     .Use<LocalizeLockDetailsPriceCustomization>()
                                                     .Use<NewLockDetailCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Order>()
                                                     .Use<OrderValidationCustomization>()
                                                     .Use<InspectorNameCustomization>()
                                                     .Use<PrivilegesCustomization>()
                                                     .Use<WorkflowStepsCustomization>()
                                                     .Use<LockOrderByReleaseCustomization>()
                                                     .Use<LockByWorkflowCustomization>()
                                                     .Use<SignupDateCustomization>()
                                                     .UseOrdered<InactiveOrderCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<OrderFile>()
                                                     .Use<OrderFileAccessCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<OrderPosition>()
                                                     .Use<MoneySignificantDigitsNumberCustomization>()
                                                     .Use<HideChangeBindingObjectsButtonCustomization>()
                                                     .Use<InitOrderPositionDiscountCustomization>()
                                                     .UseOrdered<OrderPositionRateCustomization>()
                                                     .UseOrdered<LockOrderPositionByReleaseCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<OrderProcessingRequest>()
                                                     .Use<CheckIfUserCanCreateOrderForRequestCustomization>()
                                                     .Use<ManageRequestStateButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Phonecall>()
                                                     .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Position>()
                                                     .Use<CheckIfPositionTemplateIsReadOnlyCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Price>()
                                                     .Use<ManagePricePublicationButtonsCustomization>()
                                                     .Use<PublishedPriceCustomization>()
                                                     .Use<InactivePriceCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<PricePosition>()
                                                     .Use<InactivePricePositionCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<ReleaseInfo>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Task>()
                                                     .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Territory>()
                                                     .Use<ActiveTerritoryCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<Theme>()
                                                     .Use<ManageDefaultThemeButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<User>()
                                                     .Use<EntityIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.Config
                                                     .For<WithdrawalInfo>()
                                                     .Use<SetReadonlyCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}