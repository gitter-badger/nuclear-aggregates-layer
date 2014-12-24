using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
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
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
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
                        ViewModelCustomizationsMetadata.For<Account, IEntityViewModelBase>()
                                                       .Use<AccountIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.For<Advertisement, AdvertisementViewModel>()
                                                       .Use<AdvertisementAccessCustomization>()
                                                       .Use<DummyAdvertisementCustomization>()
                                                       .Use<SelectedToWhiteListAdvertisementCustomization>(),

                        ViewModelCustomizationsMetadata.For<AdvertisementElement, AdvertisementElementViewModel>()
                                                       .Use<AdvertisementElementFasCommentCustomization>()
                                                       .Use<CheckIfAdvertisementElementReadOnly>()
                                                       .Use<ManageAdvertisementElementWorkflowButtonsCustomizations>(),

                        ViewModelCustomizationsMetadata.For<AdvertisementTemplate, AdvertisementTemplateViewModel>()
                                                       .Use<ManageAdvertisementTemplatePublicationButtonsCustomization>()
                                                       .Use<PublishedAdvertisementTemplateCustomization>(),

                        ViewModelCustomizationsMetadata.For<Appointment, ICustomizableActivityViewModel>()
                                                       .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.For<AssociatedPosition, AssociatedPositionViewModel>()
                                                       .Use<AssociatedPositionsPriceIsDeletedCustomization>()
                                                       .Use<AssociatedPositionsPriceIsPublishedCustomization>(),

                        ViewModelCustomizationsMetadata.For<AssociatedPositionsGroup, AssociatedPositionsGroupViewModel>()
                                                       .UseOrdered<AssociatedPositionGroupIsDeletedCustomization>()
                                                       .UseOrdered<AssociatedPositionGroupsPriceIsDeletedCustomization>()
                                                       .UseOrdered<AssociatedPositionGroupsPriceIsPublishedCustomization>(),

                        ViewModelCustomizationsMetadata.For<Client, IEntityViewModelBase>()
                                                       .Use<WarnLinkToAdvAgencyExistsVmCustomization>(),

                        ViewModelCustomizationsMetadata.For<Contact, ICustomizableContactViewModel>()
                                                       .Use<BusinessModelAreaCustomization>()
                                                       .Use<ContactSalutationsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Deal, IEntityViewModelBase>()
                                                       .Use<DisableReopenDealButtonCustomization>(),

                        ViewModelCustomizationsMetadata.For<DeniedPosition, DeniedPositionViewModel>()
                                                       .UseOrdered<DeniedPositionsPriceIsPublishedCustomization>()
                                                       .UseOrdered<InactiveDeniedPositionsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Firm, ICustomizableFirmViewModel>()
                                                       .Use<ChangeTerritoryPrivilegeCustomization>()
                                                       .Use<FirmIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.For<LegalPerson, ICustomizableLegalPersonViewModel>()
                                                       .UseOrdered<LegalPersonDoesntHaveAnyProfilesCustomization>()
                                                       .UseOrdered<LegalPersonIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.For<LegalPersonProfile, ICustomizableLegalPersonProfileViewModel>()
                                                       .Use<MainLegalPersonProfileCustomization>(),

                        ViewModelCustomizationsMetadata.For<Letter, ICustomizableActivityViewModel>()
                                                       .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Limit, LimitViewModel>()
                                                       .Use<CheckIfLimitRecalculationAvailableCustomization>()
                                                       .Use<CheckLimitPrivilegeCustomization>()
                                                       .Use<LockLimitByWorkflowCustomization>()
                                                       .Use<ManageLimitWorkflowButtonsCustomization>()
                                                       .Use<SetLimitInspectorNameCustomization>(),

                        ViewModelCustomizationsMetadata.For<Lock, LockViewModel>()
                                                       .Use<NewLockCustomization>()
                                                       .Use<LocalizeLockStatusCustomization>(),

                        ViewModelCustomizationsMetadata.For<LockDetail, LockDetailViewModel>()
                                                       .Use<LocalizeLockDetailsPriceCustomization>()
                                                       .Use<NewLockDetailCustomization>(),

                        ViewModelCustomizationsMetadata.For<Order, ICustomizableOrderViewModel>()
                                                       .Use<OrderValidationCustomization>()
                                                       .Use<InspectorNameCustomization>()
                                                       .Use<PrivilegesCustomization>()
                                                       .Use<WorkflowStepsCustomization>()
                                                       .Use<LockOrderByReleaseCustomization>()
                                                       .Use<LockByWorkflowCustomization>()
                                                       .Use<SignupDateCustomization>()
                                                       .UseOrdered<InactiveOrderCustomization>(),

                        ViewModelCustomizationsMetadata.For<OrderFile, OrderFileViewModel>()
                                                       .Use<OrderFileAccessCustomization>(),

                        ViewModelCustomizationsMetadata.For<OrderPosition, ICustomizableOrderPositionViewModel>()
                                                       .Use<MoneySignificantDigitsNumberCustomization>()
                                                       .Use<HideChangeBindingObjectsButtonCustomization>()
                                                       .Use<InitOrderPositionDiscountCustomization>()
                                                       .UseOrdered<OrderPositionRateCustomization>()
                                                       .UseOrdered<LockOrderPositionByReleaseCustomization>(),

                        ViewModelCustomizationsMetadata.For<OrderProcessingRequest, OrderProcessingRequestViewModel>()
                                                       .Use<CheckIfUserCanCreateOrderForRequestCustomization>()
                                                       .Use<ManageRequestStateButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Phonecall, ICustomizableActivityViewModel>()
                                                       .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Position, PositionViewModel>()
                                                       .Use<CheckIfPositionTemplateIsReadOnlyCustomization>(),

                        ViewModelCustomizationsMetadata.For<Price, PriceViewModel>()
                                                       .Use<ManagePricePublicationButtonsCustomization>()
                                                       .Use<PublishedPriceCustomization>()
                                                       .Use<InactivePriceCustomization>(),

                        ViewModelCustomizationsMetadata.For<PricePosition, IEntityViewModelBase>()
                                                       .Use<InactivePricePositionCustomization>(),

                        ViewModelCustomizationsMetadata.For<Task, ICustomizableActivityViewModel>()
                                                       .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Territory, TerritoryViewModel>()
                                                       .Use<ActiveTerritoryCustomization>(),

                        ViewModelCustomizationsMetadata.For<Theme, ThemeViewModel>()
                                                       .Use<ManageDefaultThemeButtonsCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}