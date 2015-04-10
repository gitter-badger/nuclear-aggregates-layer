using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

using NuClear.Metamodeling.UI.Elements.Aspects.Features;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class CardMetadataBuilder<TEntity> : ViewModelMetadataBuilder<CardMetadataBuilder<TEntity>, CardMetadata>
        where TEntity : IEntity, IEntityKey
    {
        private readonly ImageFeatureAspect<CardMetadataBuilder<TEntity>, CardMetadata> _icon;
        private readonly IEntityType _entityName;

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

        public CardMetadataBuilder<TEntity> MainAttribute<TViewModel>(Expression<Func<TViewModel, object>> propertyNameExpression)
            where TViewModel : IAspect
        {
            AddFeatures(new MainAttributeFeature(new PropertyDescriptor<TViewModel>(propertyNameExpression)));
            return this;
        }

        public CardMetadataBuilder<TEntity> WarningOn<T>(Expression<Func<T, bool>> expression, IStringResourceDescriptor messageDescriptor)
            where T : IAspect
        {
            AddFeatures(new MessageExpressionFeature<T>(expression, messageDescriptor, MessageType.Warning));
            return this;
        }

        public CardMetadataBuilder<TEntity> WarningOn(LambdaExpressionsSequence expressionsSequence, IStringResourceDescriptor messageDescriptor)
        {
            AddFeatures(new MessageExpressionsFeature(expressionsSequence.LogicalOperation, expressionsSequence.Expressions, messageDescriptor, MessageType.Warning));
            return this;
        }

        public CardMetadataBuilder<TEntity> ErrorOn<T>(Expression<Func<T, bool>> expression, IStringResourceDescriptor messageDescriptor)
            where T : IAspect
        {
            AddFeatures(new MessageExpressionFeature<T>(expression, messageDescriptor, MessageType.CriticalError));
            return this;
        }

        public CardMetadataBuilder<TEntity> ErrorOn(LambdaExpressionsSequence expressionsSequence, IStringResourceDescriptor messageDescriptor)
        {
            AddFeatures(new MessageExpressionsFeature(expressionsSequence.LogicalOperation, expressionsSequence.Expressions, messageDescriptor, MessageType.CriticalError));
            return this;
        }

        public CardMetadataBuilder<TEntity> InfoOn<T>(Expression<Func<T, bool>> expression, IStringResourceDescriptor messageDescriptor)
            where T : IAspect
        {
            AddFeatures(new MessageExpressionFeature<T>(expression, messageDescriptor, MessageType.Info));
            return this;
        }

        public CardMetadataBuilder<TEntity> InfoOn(LambdaExpressionsSequence expressionsSequence, IStringResourceDescriptor messageDescriptor)
        {
            AddFeatures(new MessageExpressionsFeature(expressionsSequence.LogicalOperation, expressionsSequence.Expressions, messageDescriptor, MessageType.Info));
            return this;
        }

        public CardMetadataBuilder<TEntity> ReadOnly()
        {
            AddFeatures(new ReadOnlyFeature());
            return this;
        }

        public CardMetadataBuilder<TEntity> ReadOnlyOn<T>(params Expression<Func<T, bool>>[] expressions)
            where T : IAspect
        {
            AddFeatures(expressions.Select(expression => new DisableExpressionFeature<T>(expression)).ToArray());
            return this;
        }

        public CardMetadataBuilder<TEntity> ReadOnlyOn(LambdaExpressionsSequence expressionsSequence)
        {
            AddFeatures(new DisableExpressionsFeature(expressionsSequence.LogicalOperation, expressionsSequence.Expressions));
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
            if (_entityName == EntityType.Instance.None())
            {
                throw new InvalidOperationException("Entity must be specified");    
            }

            return new CardMetadata(_entityName, Features);
        }
    }
}