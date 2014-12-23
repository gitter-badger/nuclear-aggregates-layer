using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardMetadataSource : MetadataSourceBase<MetadataCardsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public CardMetadataSource()
        {
            _metadata = CardMetadatas.Settings.Aggregate(new Dictionary<Uri, IMetadataElement>(), Process);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private Dictionary<Uri, IMetadataElement> Process(Dictionary<Uri, IMetadataElement> metadata, CardMetadata element)
        {
            metadata.Add(element.Identity.Id, element);
            return metadata;
        }
    }
}