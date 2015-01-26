using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardMetadataBuilder : ViewModelMetadataBuilder<CardMetadataBuilder, CardMetadata>
    {
        private IEntityType _entityName;

        public CardMetadataBuilder For<TEntity>()
            where TEntity : class, IEntity
        {
            _entityName = typeof(TEntity).AsEntityName();
            return this;
        }

        protected override CardMetadata Create()
        {
            return new CardMetadata(_entityName, Features);
        }
    }
}
