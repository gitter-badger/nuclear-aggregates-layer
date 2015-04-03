using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardMetadata : ViewModelMetadata<CardMetadata, CardMetadataBuilder>
    {
        private readonly IEntityType _entity;
        private readonly IMetadataElementIdentity _identity;

        public CardMetadata(IEntityType entity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entity = entity;
            _identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(_entity.ToString()).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public IEntityType Entity
        {
            get { return _entity; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
        }
    }
}