﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel
{
    public abstract class ViewModelMetadata : MetadataElement, IViewModelMetadata
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

        public UIElementMetadata[] RelatedItems
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

        public UIElementMetadata[] ActionsDescriptors
        {
            get
            {
                return _actionsFeature.Value != null ? _actionsFeature.Value.ActionsDescriptors : new UIElementMetadata[0];
            }
        }
    }

    public abstract class ViewModelMetadata<TElement, TBuilder> : ViewModelMetadata
        where TElement : ViewModelMetadata<TElement, TBuilder>
        where TBuilder : ViewModelMetadataBuilder<TBuilder, TElement>, new()
    {
        protected ViewModelMetadata(IEnumerable<IMetadataFeature> features) : base(features)
        {
        }

        public static TBuilder Config
        {
            get
            {
                return new TBuilder();
            }
        }
    }
}
