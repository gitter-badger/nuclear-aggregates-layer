using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Ukraine
{
    public sealed class UkraineCardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>, IUkraineAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public UkraineCardMetadataSource()
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
                        CardMetadata.For<Deal>()
                                    .MVVM.Bind<IDealViewModel>("~/Views/CreateOrUpdate/Ukraine/Deal.cshtml"),
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}