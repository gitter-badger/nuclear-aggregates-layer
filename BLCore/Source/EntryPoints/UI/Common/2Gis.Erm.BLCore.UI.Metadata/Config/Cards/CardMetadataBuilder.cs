using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class CardMetadataBuilder<TEntity> : ViewModelMetadataBuilder<CardMetadataBuilder<TEntity>, CardMetadata>
        where TEntity : IEntity, IEntityKey
    {
        private readonly ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> _icon;
        private readonly EntityName _entityName;

        public CardMetadataBuilder()
        {
            _icon = new ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata>(this);
            _entityName = typeof(TEntity).AsEntityName();
        }

        public ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> Icon
        {
            get { return _icon; }
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

        public CardMetadataBuilder<TEntity> ReadOnlyOn<T>(params Expression<Func<T, bool>>[] expressions)
            where T : IEntityViewModelAbstract<TEntity>
        {
            AddFeatures(expressions.Select(expression => new DisableExpressionFeature<T>(expression)).ToArray());
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