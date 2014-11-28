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
    public sealed class CardMetadataBuilder : ViewModelMetadataBuilder<CardMetadataBuilder, CardMetadata>
    {
        private readonly TitleFeatureAspect<CardMetadataBuilder, CardMetadata> _title;
        private readonly ImageFeatureAspect<CardMetadataBuilder, CardMetadata> _icon;
        private readonly RelatedItemsFeatureAspect<CardMetadataBuilder, CardMetadata> _relatedItems;
        private readonly ActionsFeatureAspect<CardMetadataBuilder, CardMetadata> _actions;

        private EntityName _entityName;

        public CardMetadataBuilder()
        {
            _title = new TitleFeatureAspect<CardMetadataBuilder, CardMetadata>(this);
            _icon = new ImageFeatureAspect<CardMetadataBuilder, CardMetadata>(this);
            _relatedItems = new RelatedItemsFeatureAspect<CardMetadataBuilder, CardMetadata>(this);
            _actions = new ActionsFeatureAspect<CardMetadataBuilder, CardMetadata>(this);
        }

        public TitleFeatureAspect<CardMetadataBuilder, CardMetadata> Title
        {
            get { return _title; }
        }

        public ImageFeatureAspect<CardMetadataBuilder, CardMetadata> Icon
        {
            get { return _icon; }
        }

        public new RelatedItemsFeatureAspect<CardMetadataBuilder, CardMetadata> RelatedItems
        {
            get { return _relatedItems; }
        }

        public new ActionsFeatureAspect<CardMetadataBuilder, CardMetadata> Actions
        {
            get { return _actions; }
        }

        public CardMetadataBuilder EntityLocalization<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            AddFeatures(new EntityNameLocalizationFeature(StringResourceDescriptor.Create(resourceKeyExpression)));
            return this;
        }

        public CardMetadataBuilder WithAdminTab()
        {
            AddFeatures(new PartFeature(ResourceTitleDescriptor.Create(() => BLResources.AdministrationTabTitle), new StaticTitleDescriptor("AdministrationTab")));
            return this;
        }

        public CardMetadataBuilder WithComments()
        {
            AddFeatures(new PartFeature(
                
                // COMMENT {all, 20.11.2014}: Эта строчка есть и в клиентских ресурсах. Есть подозрение, что источник должен остаться один
                ResourceTitleDescriptor.Create(() => BLResources.Notes),
                new StaticTitleDescriptor("notesTab")));
            return this;
        }

        public CardMetadataBuilder For<TEntity>()
            where TEntity : IEntity
        {
            _entityName = typeof(TEntity).AsEntityName();
            return new CardMetadataBuilder();
        }

        public CardMetadataBuilder For(EntityName entityName)
        {
            _entityName = entityName;
            return new CardMetadataBuilder();
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