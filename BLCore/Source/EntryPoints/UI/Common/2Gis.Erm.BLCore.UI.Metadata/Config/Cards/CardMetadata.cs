using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class CardMetadata : ViewModelMetadata,
                                       ITitledElement,
                                       IRelatedItemsHost,
                                       IActionsContained,
                                       IPartsContainerElement,
                                       IImageBoundElement
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

        public ITitleDescriptor TitleDescriptor
        {
            get
            {
                var feature = Features.OfType<TitleFeature>().SingleOrDefault();
                return feature != null ? feature.TitleDescriptor : null;
            }
        }

        public IStringResourceDescriptor EntityLocalizationDescriptor
        {
            get
            {
                var localizationFeature = Features.OfType<EntityNameLocalizationFeature>().SingleOrDefault();
                return localizationFeature != null ? localizationFeature.Descriptor : null;
            }
        }

        public bool HasRelatedItems
        {
            get { return Features.OfType<RelatedItemsFeature>().Any(); }
        }

        public UIElementMetadata[] RelatedItems
        {
            get
            {
                return Features.OfType<RelatedItemsFeature>().SelectMany(x => x.RelatedItems).ToArray();
            }
        }

        public bool HasActions { get; private set; }

        public UIElementMetadata[] ActionsDescriptors
        {
            get { return Features.OfType<ActionsFeature>().SelectMany(x => x.ActionsDescriptors).ToArray(); }
        }

        public bool HasParts
        {
            get { return Features.OfType<IPartFeature>().Any(); }
        }

        public IPartFeature[] Parts
        {
            get { return Features.OfType<IPartFeature>().ToArray(); }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
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
            where TEntity : IEntity
        {
            return new CardMetadataBuilder<TEntity>();
        }
    }
}
