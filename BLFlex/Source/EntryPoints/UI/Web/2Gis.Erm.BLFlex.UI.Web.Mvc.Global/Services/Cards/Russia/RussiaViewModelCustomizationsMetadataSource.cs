using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.Clients;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.LegalPersonProfiles;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.Orders;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia
{
    public sealed class RussiaViewModelCustomizationsMetadataSource : MetadataSourceBase<ViewModelCustomizationsIdentity>, IRussiaAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public RussiaViewModelCustomizationsMetadataSource()
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
                        ViewModelCustomizationsMetadata.For<Client, ClientViewModel>()
                                                       .Use<EditIsAdvertisingAgencyViewModelCustomization>(),

                        ViewModelCustomizationsMetadata.For<LegalPersonProfile, LegalPersonProfileViewModel>()
                                                       .Use<LegalPersonProfileDisableDocumentsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Order, EntityViewModelBase<Order>>()
                                                       .Use<PrintFormsCustomization>()
                                                       .Use<ManageDocumentsDebtCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}
