using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public sealed class CardMetadata2ViewModelPropertiesMapper : AbstractMetadata2ViewModelPropertiesMapper<CardStructure>
    {
        private delegate bool Metadata2ViewModelPropertyTransformer(
            EntityProperty metadataProperty,
            IUseCase useCase,
            IViewModelIdentity hostingViewModelIdentity,
            out IViewModelProperty transformedProperty);

        private readonly IEntityPropertiesProvider _entityPropertiesProvider;
        private readonly ILookupFactory _lookupFactory;
        private readonly IEnumerable<Metadata2ViewModelPropertyTransformer> _propertyTransformers;

        public CardMetadata2ViewModelPropertiesMapper(IEntityPropertiesProvider entityPropertiesProvider, ILookupFactory lookupFactory)
        {
            _entityPropertiesProvider = entityPropertiesProvider;
            _lookupFactory = lookupFactory;
            _propertyTransformers = new Metadata2ViewModelPropertyTransformer[] { LookupTransformer, CommonTransformer };
        }

        protected override IEnumerable<IViewModelProperty> GetViewModelProperties(IUseCase useCase, CardStructure descriptor, IViewModelIdentity targetViewModelIdentity)
        {
            var metadataProperties = _entityPropertiesProvider.GetProperties(descriptor.Identity.EntityName);
            if (metadataProperties == null)
            {
                return new IViewModelProperty[0];
            }
            
            var transformedProperties = new List<IViewModelProperty>();
            foreach (var metadataProperty in metadataProperties)
            {
                foreach (var processor in _propertyTransformers)
                {
                    IViewModelProperty transformedProperty;
                    if (processor(metadataProperty, useCase, targetViewModelIdentity, out transformedProperty))
                    {
                        transformedProperties.Add(transformedProperty);
                        break;
                    }
                }
            }

            return transformedProperties;
        }

        private bool LookupTransformer(
            EntityProperty metadataProperty, 
            IUseCase useCase, 
            IViewModelIdentity hostingViewModelIdentity, 
            out IViewModelProperty transformedProperty)
        {
            transformedProperty = null;

            var feature = metadataProperty.Features.OfType<LookupPropertyFeature>().SingleOrDefault();
            if (feature == null)
            {
                return false;
            }

            var lookupViewModel = _lookupFactory.Create(useCase, hostingViewModelIdentity, feature);
            transformedProperty = 
                new MetadataAndValueViewModelProperty(
                    metadataProperty.Name,
                    lookupViewModel.GetType(),
                    lookupViewModel,
                    metadataProperty.Features);
            return true;
        }

        private bool CommonTransformer(
            EntityProperty metadataProperty,
            IUseCase useCase,
            IViewModelIdentity hostingViewModelIdentity,
            out IViewModelProperty transformedProperty)
        {
            transformedProperty = new MetadataOnlyViewModelProperty(metadataProperty.Name, metadataProperty.Type, metadataProperty.Features);
            return true;
        }
    }
}