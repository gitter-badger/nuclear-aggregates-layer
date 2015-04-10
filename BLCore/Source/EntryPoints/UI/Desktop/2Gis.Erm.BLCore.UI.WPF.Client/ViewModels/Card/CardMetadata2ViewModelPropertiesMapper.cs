using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public sealed class CardMetadata2ViewModelPropertiesMapper : AbstractMetadata2ViewModelPropertiesMapper<CardMetadata>
    {
        private delegate bool Metadata2ViewModelPropertyTransformer(
            EntityPropertyMetadata metadataPropertyMetadata,
            IUseCase useCase,
            IViewModelIdentity hostingViewModelIdentity,
            out IViewModelProperty transformedProperty);

        private readonly IMetadataProvider _metadataProvider;
        private readonly ILookupFactory _lookupFactory;
        private readonly IEnumerable<Metadata2ViewModelPropertyTransformer> _propertyTransformers;

        public CardMetadata2ViewModelPropertiesMapper(IMetadataProvider metadataProvider, ILookupFactory lookupFactory)
        {
            _metadataProvider = metadataProvider;
            _lookupFactory = lookupFactory;
            _propertyTransformers = new Metadata2ViewModelPropertyTransformer[] { LookupTransformer, CommonTransformer };
        }

        protected override IEnumerable<IViewModelProperty> GetViewModelProperties(IUseCase useCase, CardMetadata metadata, IViewModelIdentity targetViewModelIdentity)
        {
            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataEntitiesIdentity>(metadata.Entity.Description);

            IMetadataElement propertiesContainer;
            if (!_metadataProvider.TryGetMetadata(metadataId, out propertiesContainer))
            {
                return new IViewModelProperty[0];
            }

            var transformedProperties = new List<IViewModelProperty>();
            foreach (var metadataProperty in propertiesContainer.Elements<EntityPropertyMetadata>())
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
            EntityPropertyMetadata metadataPropertyMetadata, 
            IUseCase useCase, 
            IViewModelIdentity hostingViewModelIdentity, 
            out IViewModelProperty transformedProperty)
        {
            transformedProperty = null;

            var feature = metadataPropertyMetadata.Features<LookupPropertyFeature>().SingleOrDefault();
            if (feature == null)
            {
                return false;
            }

            var lookupViewModel = _lookupFactory.Create(useCase, hostingViewModelIdentity, feature);
            transformedProperty = 
                new MetadataAndValueViewModelProperty(
                    metadataPropertyMetadata.Name,
                    lookupViewModel.GetType(),
                    lookupViewModel,
                    metadataPropertyMetadata.Features<IPropertyFeature>());
            return true;
        }

        private bool CommonTransformer(
            EntityPropertyMetadata metadataPropertyMetadata,
            IUseCase useCase,
            IViewModelIdentity hostingViewModelIdentity,
            out IViewModelProperty transformedProperty)
        {
            transformedProperty = new MetadataOnlyViewModelProperty(metadataPropertyMetadata.Name, metadataPropertyMetadata.Type, metadataPropertyMetadata.Features<IPropertyFeature>());
            return true;
        }
    }
}