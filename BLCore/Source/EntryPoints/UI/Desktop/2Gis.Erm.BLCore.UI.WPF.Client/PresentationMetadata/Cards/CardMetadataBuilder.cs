using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardMetadataBuilder : ViewModelMetadataBuilder<CardMetadataBuilder, CardMetadata>
    {
        private EntityName _entityName;

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
