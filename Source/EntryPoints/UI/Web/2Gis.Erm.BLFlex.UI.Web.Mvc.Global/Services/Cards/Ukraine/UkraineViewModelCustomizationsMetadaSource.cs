using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Ukraine
{
    public class UkraineViewModelCustomizationsMetadaSource : MetadataSourceBase<ViewModelCustomizationsIdentity>, IUkraineAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public UkraineViewModelCustomizationsMetadaSource()
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
                                                     .For<Order>()
                                                     .Use<MultiCulturePrintFormsCustomization>(),

                        ViewModelCustomizationsMetada.Config
                                                     .For<LegalPersonProfile>()
                                                     .Use<UkraineLegalPersonProfileDisableDocumentsCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}