﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositionGroups;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Clients;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Contacts;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.DeniedPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersons;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LockDetails;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Locks;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Positions;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Themes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
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
                        ViewModelCustomizationsMetadata.For<Advertisement, AdvertisementViewModel>()
                                                       .Use<AdvertisementAccessCustomization>(),

                        ViewModelCustomizationsMetadata.For<AdvertisementElement, AdvertisementElementViewModel>()
                                                       .Use<AdvertisementElementFasCommentCustomization>(),

                        ViewModelCustomizationsMetadata.For<AssociatedPositionsGroup, AssociatedPositionsGroupViewModel>()
                                                       .UseOrdered<AssociatedPositionGroupIsDeletedCustomization>()
                                                       .UseOrdered<AssociatedPositionGroupsPriceIsDeletedCustomization>()
                                                       .UseOrdered<AssociatedPositionGroupsPriceIsPublishedCustomization>(),

                        ViewModelCustomizationsMetadata.For<Client, IEntityViewModelBase>()
                                                       .Use<WarnLinkToAdvAgencyExistsVmCustomization>(),

                        ViewModelCustomizationsMetadata.For<Contact, ICustomizableContactViewModel>()
                                                       .Use<BusinessModelAreaCustomization>()
                                                       .Use<ContactSalutationsCustomization>(),

                        ViewModelCustomizationsMetadata.For<DeniedPosition, DeniedPositionViewModel>()
                                                       .UseOrdered<DeniedPositionsPriceIsPublishedCustomization>()
                                                       .UseOrdered<InactiveDeniedPositionsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Firm, ICustomizableFirmViewModel>()
                                                       .Use<FirmIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.For<LegalPerson, ICustomizableLegalPersonViewModel>()
                                                       .UseOrdered<LegalPersonDoesntHaveAnyProfilesCustomization>()
                                                       .UseOrdered<LegalPersonIsInactiveCustomization>(),

                        ViewModelCustomizationsMetadata.For<Limit, LimitViewModel>()
                                                       .Use<CheckIfLimitRecalculationAvailableCustomization>()
                                                       .Use<CheckLimitPrivilegeCustomization>()
                                                       .Use<SetLimitInspectorNameCustomization>(),

                        ViewModelCustomizationsMetadata.For<Lock, LockViewModel>()
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
                                                       .Use<SignupDateCustomization>()
                                                       .UseOrdered<InactiveOrderCustomization>(),

                        ViewModelCustomizationsMetadata.For<OrderPosition, ICustomizableOrderPositionViewModel>()
                                                       .Use<MoneySignificantDigitsNumberCustomization>()
                                                       .Use<HideChangeBindingObjectsButtonCustomization>()
                                                       .Use<InitOrderPositionDiscountCustomization>()
                                                       .UseOrdered<OrderPositionRateCustomization>()
                                                       .UseOrdered<LockOrderPositionByReleaseCustomization>(),

                        ViewModelCustomizationsMetadata.For<Position, PositionViewModel>()
                                                       .Use<CheckIfPositionTemplateIsReadOnlyCustomization>(),

                        ViewModelCustomizationsMetadata.For<Theme, ThemeViewModel>()
                                                       .Use<ManageDefaultThemeButtonsCustomization>()
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}