﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Accounts;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Activities;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementTemplates;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Clients;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Deals;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.DeniedPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersonProfiles;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersons;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
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
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}