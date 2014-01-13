using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardStructuresProvider : ICardStructuresProvider
    {
        private readonly IEntityPropertiesProvider _entitySettingsProvider;
        private readonly IReadOnlyDictionary<EntityName, CardStructure> _entity2CardMap; 

        public CardStructuresProvider(IEntityPropertiesProvider entitySettingsProvider)
        {
            _entitySettingsProvider = entitySettingsProvider;
            _entity2CardMap = ValidateByEntitiesConfig(CardStructures.Settings).ToDictionary(descriptor => descriptor.Identity.EntityName);
        }

        public bool TryGetDescriptor(EntityName entityName, out CardStructure descriptor)
        {
            return _entity2CardMap.TryGetValue(entityName, out descriptor);
        }

        public CardStructure[] Structures
        {
            get
            {
                return _entity2CardMap.Values.ToArray();
            }
        }

        private IEnumerable<CardStructure> ValidateByEntitiesConfig(CardStructure[] cardDependencies)
        {
            foreach (var descriptor in cardDependencies)
            {
                var entitySettings = _entitySettingsProvider.GetProperties(descriptor.Identity.EntityName);
                if (!entitySettings.Any())
                {
                    if (descriptor.ElementFeatures.Any(feature => feature is DynamicPropertiesFeature))
                    {
                        throw new InvalidOperationException("Can't use dynamic properties feature for entity without configured entity properties. EntityName: " + descriptor.Identity.EntityName);
                    }
                }
            }

            return cardDependencies;
        }
    }
}