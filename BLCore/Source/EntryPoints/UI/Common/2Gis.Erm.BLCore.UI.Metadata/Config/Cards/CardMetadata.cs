using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    // TODO {all, 20.11.2014}: Перекликается с реализацией для WPF. При возобновлении работы над WPF. Код нужно будет как-то объединить.
    public sealed class CardMetadata : MetadataElement<CardMetadata, CardMetadataBuilder<IEntity>>, 
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

        public ITitleDescriptor TitleDescriptor { get; private set; }
        public bool HasRelatedItems { get; private set; }
        public UiElementMetadata[] RelatedItems { get; private set; }
        public bool HasActions { get; private set; }
        public UiElementMetadata[] ActionsDescriptors { get; private set; }

        public bool HasParts
        {
            get { return Features.OfType<IPartFeature>().Any(); }
        }

        public IPartFeature[] Parts
        {
            get { return Features.OfType<IPartFeature>().ToArray(); }
        }

        public static CardMetadataBuilder<TEntity> For<TEntity>()
            where TEntity : IEntity
        {
            return new CardMetadataBuilder<TEntity>();
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
        }

        public IImageDescriptor ImageDescriptor { get; private set; }
    }
}
