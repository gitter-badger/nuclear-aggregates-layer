using System;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Validators;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;

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
                var entityMetadataId = IdBuilder.For<MetadataEntitiesIdentity>(cardStructure.Entity.ToString());
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