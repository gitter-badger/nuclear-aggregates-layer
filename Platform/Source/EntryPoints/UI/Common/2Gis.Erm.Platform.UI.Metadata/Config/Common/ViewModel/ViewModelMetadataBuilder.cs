using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel
{
    public abstract class ViewModelMetadataBuilder<TBuilder, TElement> : MetadataElementBuilder<TBuilder, TElement>
        where TBuilder : ViewModelMetadataBuilder<TBuilder, TElement>, new()
        where TElement : ViewModelMetadata
    {
        private readonly TitleFeatureAspect<TBuilder, TElement> _title;
        private readonly OperationFeatureAspect<TBuilder, TElement> _operation;
        private readonly ViewModelViewMappingAspect<TBuilder, TElement> _mvvm;
        private readonly ViewModelPartsFeatureAspect<TBuilder, TElement> _parts;
        private readonly RelatedItemsFeatureAspect<TBuilder, TElement> _relatedItems;
        private readonly ValidatorViewModelFeatureAspect<TBuilder, TElement> _validator;
        private readonly ActionsFeatureAspect<TBuilder, TElement> _actions;

        protected ViewModelMetadataBuilder()
        {
            _title = new TitleFeatureAspect<TBuilder, TElement>(this);
            _operation = new OperationFeatureAspect<TBuilder, TElement>(this);
            _mvvm = new ViewModelViewMappingAspect<TBuilder, TElement>(this);
            _parts = new ViewModelPartsFeatureAspect<TBuilder, TElement>(this);
            _relatedItems = new RelatedItemsFeatureAspect<TBuilder, TElement>(this);
            _validator = new ValidatorViewModelFeatureAspect<TBuilder, TElement>(this);
            _actions = new ActionsFeatureAspect<TBuilder, TElement>(this);
        }

        public TitleFeatureAspect<TBuilder, TElement> Title
        {
            get { return _title; }
        }

        public OperationFeatureAspect<TBuilder, TElement> Operation
        {
            get { return _operation; }
        }

        public ViewModelViewMappingAspect<TBuilder, TElement> MVVM
        {
            get { return _mvvm; }
        }

        public ViewModelPartsFeatureAspect<TBuilder, TElement> Parts
        {
            get { return _parts; }
        }

        public RelatedItemsFeatureAspect<TBuilder, TElement> RelatedItems
        {
            get { return _relatedItems; }
        }

        public ValidatorViewModelFeatureAspect<TBuilder, TElement> Validator
        {
            get { return _validator; }
        }

        public ActionsFeatureAspect<TBuilder, TElement> Actions
        {
            get { return _actions; }
        }

        public TBuilder WithDynamicProperties()
        {
            WithFeatures(new DynamicPropertiesFeature());
            return ReturnBuilder();
        }

        public TBuilder Localizator(params Type[] resourceManagerHostTypes)
        {
            WithFeatures(new LocalizeViewModelFeature { ResourceManagerHostTypes = resourceManagerHostTypes });
            return ReturnBuilder();
        }
    }
}
