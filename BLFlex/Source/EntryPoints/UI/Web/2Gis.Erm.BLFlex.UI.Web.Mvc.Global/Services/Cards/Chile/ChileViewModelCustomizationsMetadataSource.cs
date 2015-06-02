using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Chile
{
    public class ChileViewModelCustomizationsMetadataSource : MetadataSourceBase<ViewModelCustomizationsIdentity>, IChileAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ChileViewModelCustomizationsMetadataSource()
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
                        ViewModelCustomizationsMetadata.For<Order, EntityViewModelBase<Order>>()
                                                       .Use<ChilePrintFormsCustomization>(),

                        ViewModelCustomizationsMetadata.For<LegalPersonProfile, ChileLegalPersonProfileViewModel>()
                                                       .Use<ChileLegalPersonProfileDisableDocumentsCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}
