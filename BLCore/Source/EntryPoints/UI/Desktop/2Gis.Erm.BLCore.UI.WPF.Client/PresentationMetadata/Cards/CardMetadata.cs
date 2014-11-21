using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardMetadata : ViewModelMetadata<CardMetadata, CardMetadataBuilder>
    {
        private readonly EntityName _entity;
        private readonly IMetadataElementIdentity _identity;

        public CardMetadata(EntityName entity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entity = entity;
            _identity = IdBuilder.For<MetadataCardsIdentity>(_entity.ToString()).AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public EntityName Entity
        {
            get { return _entity; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
        }
    }
}