using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel
{
    public abstract class ViewModelStructure<TElement, TIdentity, TBuilder> : ConfigElement<TElement, TIdentity, TBuilder>, IViewModelStructure
        where TElement : ConfigElement<TElement, TIdentity, TBuilder> 
        where TIdentity : class, IConfigElementIdentity 
        where TBuilder : ConfigElementBuilder<TBuilder, TElement>, new()
    {
        private readonly Lazy<TIdentity> _identity;
        private readonly Lazy<IViewModelPartsFeature> _partsFeature;
        private readonly Lazy<IRelatedItemsFeature> _relatedItemsFeature;
        private readonly Lazy<CompositeValidatorViewModelFeature> _validatorsFeature;
        private readonly Lazy<BatchOperationFeature> _operationFeature;
        private readonly Lazy<ActionsFeature> _actionsFeature;

        protected ViewModelStructure(IEnumerable<IConfigFeature> features)
            : base(features)
        {
            _identity = new Lazy<TIdentity>(GetIdentity);
            _partsFeature = new Lazy<IViewModelPartsFeature>(() => ElementFeatures.OfType<IViewModelPartsFeature>().SingleOrDefault());
            _relatedItemsFeature = new Lazy<IRelatedItemsFeature>(() => ElementFeatures.OfType<IRelatedItemsFeature>().SingleOrDefault());
            _validatorsFeature = new Lazy<CompositeValidatorViewModelFeature>(() => ElementFeatures.OfType<CompositeValidatorViewModelFeature>().SingleOrDefault());
            _operationFeature = new Lazy<BatchOperationFeature>(() => ElementFeatures.OfType<BatchOperationFeature>().SingleOrDefault());
            _actionsFeature = new Lazy<ActionsFeature>(() => ElementFeatures.OfType<ActionsFeature>().SingleOrDefault());
        }

        #region IConfigElement

        public override IConfigElementIdentity ElementIdentity
        {
            get
            {
                return _identity.Value;
            }
        }

        public override TIdentity Identity
        {
            get
            {
                return _identity.Value;
            }
        }

        #endregion

        public IViewModelViewMapping ViewModelViewMapping
        {
            get
            {
                var feature = ElementFeatures.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
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

        public HierarchyElement[] RelatedItems 
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
        
        public ITitleDescriptor TitleDescriptor {
            get
            {
                var feature = ElementFeatures.OfType<TitleFeature>().SingleOrDefault();
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

        public IEnumerable<IBoundOperationFeature> OperationFeatures
        {
            get
            {
                return _operationFeature.Value != null ? _operationFeature.Value.OperationFeatures : Enumerable.Empty<IBoundOperationFeature>();
            }
        }
        
        public bool HasActions 
        {
            get
            {
                return _actionsFeature.Value != null && _actionsFeature.Value.ActionsDescriptors.Any();
            }
        }

        public HierarchyElement[] ActionsDescriptors 
        {
            get
            {
                return _actionsFeature.Value != null ? _actionsFeature.Value.ActionsDescriptors : new HierarchyElement[0];
            }
        }
        
        protected abstract TIdentity GetIdentity();
        
    }
}
