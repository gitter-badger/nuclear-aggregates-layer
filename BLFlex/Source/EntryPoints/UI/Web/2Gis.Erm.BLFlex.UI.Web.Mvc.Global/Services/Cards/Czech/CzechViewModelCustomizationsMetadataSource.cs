﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Czech
{
    public class CzechViewModelCustomizationsMetadataSource : MetadataSourceBase<ViewModelCustomizationsIdentity>, ICzechAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public CzechViewModelCustomizationsMetadataSource()
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
                        ViewModelCustomizationsMetadata.For<LegalPersonProfile, CzechLegalPersonProfileViewModel>()
                                                       .Use<CzechLegalPersonProfileDisableDocumentsCustomization>(),

                        ViewModelCustomizationsMetadata.For<Order, ICustomizableOrderViewModel>()
                                                       .Use<CzechPrintFormsCustomization>(),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}
