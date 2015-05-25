using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;

using NuClear.Metamodeling.UI.Elements.Aspects.Features;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Extensions
{
    public static class CardMetadataBuilderExtensions
    {
        public static CardMetadataBuilder<TEntity> WithDefaultMainAttribute<TEntity>(this CardMetadataBuilder<TEntity> builder)
            where TEntity : IEntityKey, IEntity
        {
            builder.WithFeatures(new MainAttributeFeature(new PropertyDescriptor<IIdentityAspect>(x => x.Id)));
            return builder;
        }
    }
}