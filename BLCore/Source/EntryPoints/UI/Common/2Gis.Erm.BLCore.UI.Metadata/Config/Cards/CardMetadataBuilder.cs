using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    // TODO {all, 20.11.2014}: Перекликается с реализацией для WPF. При возобновлении работы над WPF. Код нужно будет как-то объединить.
    public sealed class CardMetadataBuilder<TEntity> : MetadataElementBuilder<CardMetadataBuilder<TEntity>, CardMetadata>
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

        public RelatedItemsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> RelatedItems
        {
            get { return _relatedItems; }
        }

        public ActionsFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> Actions
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
            AddFeatures(new PartFeature(ResourceTitleDescriptor.Create(() => BLResources.AdministrationTabTitle), new StaticTitleDescriptor("AdministrationTab")));
            return this;
        }

        public CardMetadataBuilder<TEntity> WithComments()
        {
            AddFeatures(new PartFeature(
                
                // COMMENT {all, 20.11.2014}: Эта строчка есть и в клиентских ресурсах. Есть подозрение, что источник должен остаться один
                ResourceTitleDescriptor.Create(() => BLResources.Notes),
                new StaticTitleDescriptor("notesTab")));
            return this;
        }

        protected override CardMetadata Create()
        {
            return new CardMetadata(_entityName, Features);
        }
    }
}