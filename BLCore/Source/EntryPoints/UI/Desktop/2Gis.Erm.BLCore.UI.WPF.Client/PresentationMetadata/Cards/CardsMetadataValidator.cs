using System;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Validators;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class CardsMetadataValidator : MetadataValidatorBase<MetadataCardsIdentity>
    {
        public CardsMetadataValidator(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }

        protected override bool IsValidImpl(MetadataSet targetMetadata, out string report)
        {
            MetadataSet entitiesMetadata;
            if (!MetadataProvider.TryGetMetadata<MetadataEntitiesIdentity>(out entitiesMetadata))
            {
                report = "Can't get required metadata of kind " + typeof(MetadataEntitiesIdentity);
                return false;
            }

            var aggregatedReport = new StringBuilder();

            foreach (var cardStructure in targetMetadata.Metadata.Values.Cast<CardMetadata>().Where(cs => cs.Uses<DynamicPropertiesFeature>()))
            {
                IMetadataElement propertiesContainer;
                var entityMetadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataEntitiesIdentity>(cardStructure.Entity.Description);
                if (!entitiesMetadata.Metadata.TryGetValue(entityMetadataId, out propertiesContainer) 
                    || !propertiesContainer.Elements<EntityPropertyMetadata>().Any())
                {
                    throw new InvalidOperationException("Can't use dynamic properties feature for entity without configured entity properties. EntityName: " + cardStructure.Entity);
                }
            }

            report = aggregatedReport.Length > 0 ? aggregatedReport.ToString() : null;
            return report == null;
        }
    }
}