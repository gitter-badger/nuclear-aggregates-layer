using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class CardMetadata : ViewModelMetadata,
                                       IPartsContainerElement,
                                       IImageBoundElement
    {
        private readonly IEntityType _entity;
        private readonly IMetadataElementIdentity _identity;

        public CardMetadata(IEntityType entity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entity = entity;
            _identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(_entity.Description).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public IEntityType Entity
        {
            get { return _entity; }
        }

        public IStringResourceDescriptor EntityLocalizationDescriptor
        {
            get
            {
                var localizationFeature = Features.OfType<EntityNameLocalizationFeature>().SingleOrDefault();
                return localizationFeature != null ? localizationFeature.Descriptor : null;
            }
        }

        public new bool HasParts
        {
            get { return Features.OfType<IPartFeature>().Any(); }
        }

        public new IPartFeature[] Parts
        {
            get { return Features.OfType<IPartFeature>().ToArray(); }
        }

        public IImageDescriptor ImageDescriptor
        {
            get
            {
                var feature = Features.OfType<ImageFeature>().SingleOrDefault();
                return feature != null ? feature.ImageDescriptor : null;
            }
        }

        public static CardMetadataBuilder<TEntity> For<TEntity>()
            where TEntity : IEntity, IEntityKey
        {
            return new CardMetadataBuilder<TEntity>();
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
        }
    }
}
