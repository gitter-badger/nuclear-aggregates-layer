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
    public class ViewModelCustomizationsMetadaSource : MetadataSourceBase<ViewModelCustomizationsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ViewModelCustomizationsMetadaSource()
        {
            _metadata = InitializeMetadataContainer();
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private static IReadOnlyDictionary<Uri, IMetadataElement> InitializeMetadataContainer()
        {
            IReadOnlyCollection<ViewModelCustomizationsMetada> metadataContainer =
                new ViewModelCustomizationsMetada[]
                    {
                        ViewModelCustomizationsMetada.Config
                                                     .For<Client>()
                                                     .Use<WarnLinkToAdvAgencyExistsVmCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<LegalPerson>()
                                                     .UseWithOrder<LegalPersonDoesntHaveAnyProfilesCustomization>(1)
                                                     .UseWithOrder<LegalPersonIsInactiveCustomization>(2),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Order>()
                                                     .Use<OrderValidationCustomization>()
                                                     .Use<InspectorNameCustomization>()
                                                     .Use<PrivilegesCustomization>()
                                                     .Use<WorkflowStepsCustomization>()
                                                     .Use<LockOrderByReleaseCustomization>()
                                                     .Use<LockByWorkflowCustomization>()
                                                     .Use<SignupDateCustomization>()
                                                     .UseWithOrder<InactiveOrderCustomization>(1),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Deal>()
                                                     .Use<DisableReopenDealButtonCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Firm>()
                                                     .Use<ChangeTerritoryPrivilegeCustomization>()
                                                     .Use<FirmIsInactiveCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<OrderPosition>()
                                                     .Use<MoneySignificantDigitsNumberCustomization>()
                                                     .Use<HideChangeBindingObjectsButtonCustomization>()
                                                     .Use<InitOrderPositionDiscountCustomization>()
                                                     .UseWithOrder<OrderPositionRateCustomization>(1)
                                                     .UseWithOrder<LockOrderPositionByReleaseCustomization>(2),

                        ViewModelCustomizationsMetada.Config
                                                     .For<OrderFile>()
                                                     .Use<OrderFileAccessCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Account>()
                                                     .Use<AccountIsInactiveCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<LegalPersonProfile>()
                                                     .Use<MainLegalPersonProfileCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Appointment>()
                                                     .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Phonecall>()
                                                     .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Task>()
                                                     .Use<DisableActivityButtonsCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<AdvertisementTemplate>()
                                                     .Use<ManageAdvertisementTemplatePublicationButtonsCustomization>()
                                                     .Use<PublishedAdvertisementTemplateCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Advertisement>()
                                                     .Use<AdvertisementAccessCustomization>()
                                                     .Use<DummyAdvertisementCustomization>()
                                                     .Use<SelectedToWhiteListAdvertisementCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Category>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<FirmContact>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<DeniedPosition>()
                                                     .UseWithOrder<DeniedPositionsPriceIsPublishedCustomization>(1)
                                                     .UseWithOrder<InactiveDeniedPositionsCustomization>(2),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Price>()
                                                     .Use<ManagePricePublicationButtonsCustomization>()
                                                     .Use<PublishedPriceCustomization>()
                                                     .Use<InactivePriceCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<PricePosition>()
                                                     .Use<InactivePricePositionCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<AssociatedPosition>()
                                                     .Use<AssociatedPositionsPriceIsDeletedCustomization>()
                                                     .Use<AssociatedPositionsPriceIsPublishedCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<AssociatedPositionsGroup>()
                                                     .UseWithOrder<AssociatedPositionGroupIsDeletedCustomization>(1)
                                                     .UseWithOrder<AssociatedPositionGroupsPriceIsDeletedCustomization>(2)
                                                     .UseWithOrder<AssociatedPositionGroupsPriceIsPublishedCustomization>(3),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Lock>()
                                                     .Use<SetReadonlyCustomization>()
                                                     .Use<NewLockCustomization>()
                                                     .Use<LocalizeLockStatusCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<LockDetail>()
                                                     .Use<LocalizeLockDetailsPriceCustomization>()
                                                     .Use<NewLockDetailCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Limit>()
                                                     .Use<CheckIfLimitRecalculationAvailableCustomization>()
                                                     .Use<CheckLimitPrivilegeCustomization>()
                                                     .Use<LockLimitByWorkflowCustomization>()
                                                     .Use<ManageLimitWorkflowButtonsCustomization>()
                                                     .Use<SetLimitInspectorNameCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Territory>()
                                                     .Use<ActiveTerritoryCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Position>()
                                                     .Use<CheckIfPositionTemplateIsReadOnlyCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<AdvertisementElement>()
                                                     .Use<AdvertisementElementFasCommentCustomization>()
                                                     .Use<CheckIfAdvertisementElementReadOnly>()
                                                     .Use<ManageAdvertisementElementWorkflowButtonsCustomizations>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<Theme>()
                                                     .Use<ManageDefaultThemeButtonsCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<OrderProcessingRequest>()
                                                     .Use<CheckIfUserCanCreateOrderForRequestCustomization>()
                                                     .Use<ManageRequestStateButtonsCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<LocalMessage>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<ReleaseInfo>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<WithdrawalInfo>()
                                                     .Use<SetReadonlyCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<User>()
                                                     .Use<EntityIsInactiveCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}