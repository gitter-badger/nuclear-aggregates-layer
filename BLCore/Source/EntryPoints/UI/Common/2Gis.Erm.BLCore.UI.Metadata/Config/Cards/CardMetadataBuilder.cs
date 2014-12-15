using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class CardMetadataBuilder<TEntity> : ViewModelMetadataBuilder<CardMetadataBuilder<TEntity>, CardMetadata>
        where TEntity : IEntity
    {
        private readonly TitleFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> _title;
        private readonly ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> _icon;
        private readonly RelatedItemsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> _relatedItems;
        private readonly ActionsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> _actions;

        private readonly EntityName _entityName;

        public CardMetadataBuilder()
        {
            _title = new TitleFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata>(this);
            _icon = new ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata>(this);
            _relatedItems = new RelatedItemsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata>(this);
            _actions = new ActionsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata>(this);
            _entityName = typeof(TEntity).AsEntityName();
        }

        public TitleFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> Title
        {
            get { return _title; }
        }

        public ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> Icon
        {
            get { return _icon; }
        }

        public new RelatedItemsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> RelatedItems
        {
            get { return _relatedItems; }
        }

        public new ActionsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> Actions
        {
            get { return _actions; }
        }

        public CardMetadataBuilder<TEntity> EntityLocalization<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            AddFeatures(new EntityNameLocalizationFeature(StringResourceDescriptor.Create(resourceKeyExpression)));
            return this;
        }

        public CardMetadataBuilder<TEntity> WithAdminTab()
        {
            AddFeatures(new ShowAdminPartFeature(),
                        new PartFeature(ResourceTitleDescriptor.Create(() => BLResources.AdministrationTabTitle), new StaticTitleDescriptor("AdministrationTab")));
            return this;
        }

        public CardMetadataBuilder<TEntity> ReadOnly()
        {
            AddFeatures(new ReadOnlyFeature());
            return this;
        }

        public CardMetadataBuilder<TEntity> WithComments()
        {
            AddFeatures(new ShowNotesFeature(),
                        new PartFeature(

                            // COMMENT {all, 20.11.2014}: Эта строчка есть и в клиентских ресурсах. Есть подозрение, что источник должен остаться один
                            ResourceTitleDescriptor.Create(() => BLResources.Notes),
                            new StaticTitleDescriptor("notesTab")));
            return this;
        }

        protected override CardMetadata Create()
        {
            if (_entityName == EntityName.None)
            {
                throw new InvalidOperationException("Entity must be specified");    
            }

            return new CardMetadata(_entityName, Features);
        }
    }
}