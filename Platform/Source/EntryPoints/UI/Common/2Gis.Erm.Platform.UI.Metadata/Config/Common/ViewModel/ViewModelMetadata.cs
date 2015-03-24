using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.Metamodeling.UI.Utils.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel
{
    public abstract class ViewModelMetadata<TElement, TBuilder> : MetadataElement<TElement, TBuilder>, IViewModelMetadata
        where TElement : MetadataElement<TElement, TBuilder> 
        where TBuilder : MetadataElementBuilder<TBuilder, TElement>, new()
    {
        private readonly Lazy<IViewModelPartsFeature> _partsFeature;
        private readonly Lazy<IRelatedItemsFeature> _relatedItemsFeature;
        private readonly Lazy<CompositeValidatorViewModelFeature> _validatorsFeature;
        private readonly Lazy<OperationsSetFeature> _operationFeature;
        private readonly Lazy<ActionsFeature> _actionsFeature;

        protected ViewModelMetadata(IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _partsFeature = new Lazy<IViewModelPartsFeature>(() => Features.OfType<IViewModelPartsFeature>().SingleOrDefault());
            _relatedItemsFeature = new Lazy<IRelatedItemsFeature>(() => Features.OfType<IRelatedItemsFeature>().SingleOrDefault());
            _validatorsFeature = new Lazy<CompositeValidatorViewModelFeature>(() => Features.OfType<CompositeValidatorViewModelFeature>().SingleOrDefault());
            _operationFeature = new Lazy<OperationsSetFeature>(() => Features.OfType<OperationsSetFeature>().SingleOrDefault());
            _actionsFeature = new Lazy<ActionsFeature>(() => Features.OfType<ActionsFeature>().SingleOrDefault());
        }

        public IViewModelViewMapping ViewModelViewMapping
        {
            get
            {
                var feature = Features.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
                return feature != null ? feature.Mapping : null;
            }
        }

        public bool HasParts 
        {
            get
            {
                return _partsFeature.Value != null && _partsFeature.Value.PartKeys.Any();
            }
        }

        public ResourceEntryKey[] Parts
        {
            get
            {
                return _partsFeature.Value != null ? _partsFeature.Value.PartKeys : new ResourceEntryKey[0];
            }
        }

        public bool HasRelatedItems
        {
            get
            {
                return _relatedItemsFeature.Value != null && _relatedItemsFeature.Value.RelatedItems.Any();
            }
        }

        public HierarchyMetadata[] RelatedItems 
        {
            get
            {
                return _relatedItemsFeature.Value != null ? _relatedItemsFeature.Value.RelatedItems : null;
            }
        }

        public bool ValidationEnabled
        {
            get
            {
                return _validatorsFeature.Value != null && _validatorsFeature.Value.Validators.Any();
            }
        }

        public IEnumerable<IValidatorViewModelFeature> Validators 
        {
            get
            {
                return _validatorsFeature.Value != null ? _validatorsFeature.Value.Validators : Enumerable.Empty<IValidatorViewModelFeature>();
            }
        }
        
        public ITitleDescriptor TitleDescriptor 
        {
            get
            {
                var feature = Features.OfType<TitleFeature>().SingleOrDefault();
                return feature != null ? feature.TitleDescriptor : null;
            }
        }

        public bool HasOperations
        {
            get
            {
                return _operationFeature.Value != null && _operationFeature.Value.OperationFeatures.Any();
            }
        }

        public IEnumerable<OperationFeature> OperationFeatures
        {
            get
            {
                return _operationFeature.Value != null ? _operationFeature.Value.OperationFeatures : Enumerable.Empty<OperationFeature>();
            }
        }
        
        public bool HasActions 
        {
            get
            {
                return _actionsFeature.Value != null && _actionsFeature.Value.ActionsDescriptors.Any();
            }
        }

        public HierarchyMetadata[] ActionsDescriptors 
        {
            get
            {
                return _actionsFeature.Value != null ? _actionsFeature.Value.ActionsDescriptors : new HierarchyMetadata[0];
            }
        }
    }
}
