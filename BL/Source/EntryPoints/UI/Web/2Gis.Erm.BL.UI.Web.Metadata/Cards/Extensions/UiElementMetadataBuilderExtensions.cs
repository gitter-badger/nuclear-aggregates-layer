using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class UIElementMetadataBuilderExtensions
    {
        public static UIElementMetadataBuilder AppendapleEntity<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : IEntity
        {
            return builder.WithFeatures(new AppendableEntityFeature(typeof(TEntity).AsEntityName()));
        }

        public static UIElementMetadataBuilder FilterToParent(this UIElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterToParentFeature());
        }

        public static UIElementMetadataBuilder FilterToParents(this UIElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterByParentsFeature());
        }

        public static UIElementMetadataBuilder DefaultDataView<TKey>(this UIElementMetadataBuilder builder, Expression<Func<TKey>> resourceKeyExpression)
        {
            return builder.WithFeatures(new DefaultDataViewFeature(StaticReflection.GetMemberName(resourceKeyExpression)));
        }

        public static UIElementMetadataBuilder ExtendedInfo(this UIElementMetadataBuilder builder, IResourceDescriptor extendedInfo)
        {
            return builder.WithFeatures(new ExtendedInfoFeature(extendedInfo));
        }

        public static UIElementMetadataBuilder JSHandler(this UIElementMetadataBuilder builder, string handlerName)
        {
            return builder.Handler.Name("scope." + handlerName);
        }
    }
}
