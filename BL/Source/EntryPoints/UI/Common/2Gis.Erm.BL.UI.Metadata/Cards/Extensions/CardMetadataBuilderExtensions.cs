using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Extensions
{
    public static class CardMetadataBuilderExtensions
    {
        public static CardMetadataBuilder<TEntity> MainAttribute<TEntity, TViewModel>(this CardMetadataBuilder<TEntity> builder, Expression<Func<TViewModel, object>> propertyNameExpression)
            where TEntity : IEntityKey, IEntity
            where TViewModel : IEntityViewModelAbstract<TEntity>
        {
            builder.WithFeatures(new CardMainAttributeFeature(new PropertyDescriptor<TViewModel>(propertyNameExpression)));
            return builder;
        }

        public static CardMetadataBuilder<TEntity> MainAttribute<TEntity>(this CardMetadataBuilder<TEntity> builder, Expression<Func<IEntityViewModelAbstract<TEntity>, object>> propertyNameExpression)
            where TEntity : IEntityKey, IEntity
        {
            builder.WithFeatures(new CardMainAttributeFeature(new PropertyDescriptor<IEntityViewModelAbstract<TEntity>>(propertyNameExpression)));
            return builder;
        }
    }
}