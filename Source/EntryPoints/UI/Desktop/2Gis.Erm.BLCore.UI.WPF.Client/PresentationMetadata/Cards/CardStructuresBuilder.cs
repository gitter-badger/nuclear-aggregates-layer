using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardStructureBuilder : ViewModelStructuresBuilder<CardStructureBuilder, CardStructure, CardStructureIdentity>
    {
        private EntityName _entityName;

        public CardStructureBuilder For(EntityName entityName)
        {
            _entityName = entityName;
            return this;
        }

        protected override CardStructure Create()
        {
            return new CardStructure(_entityName, Features);
        }
    }
}
