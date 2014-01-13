using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardStructure : ViewModelStructure<CardStructure, CardStructureIdentity, CardStructureBuilder>
    {
        private readonly EntityName _entityName;

        public CardStructure(EntityName entityName, IEnumerable<IConfigFeature> features)
            : base(features)
        {
            _entityName = entityName;
        }

        protected override CardStructureIdentity GetIdentity()
        {
            return new CardStructureIdentity(_entityName);
        }
    }
}