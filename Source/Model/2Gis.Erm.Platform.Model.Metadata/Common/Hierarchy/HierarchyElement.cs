using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Mode;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy
{    
    public sealed class HierarchyElement : ConfigElement<HierarchyElement, OrdinaryConfigElementIdentity, HierarchyElementsBuilder>, 
        ITitledElement, 
        IImageBoundElement,
        IHandlerBoundElement,
        IOperationsBoundElement,
        ISharable
    {
        private readonly OrdinaryConfigElementIdentity _identity = new OrdinaryConfigElementIdentity();
        private readonly Lazy<IHandlerFeature> _handler;
        private readonly Lazy<BatchOperationFeature> _operationFeature;
        private readonly Lazy<SharedFeature> _sharedMode;

        public HierarchyElement(IConfigFeature[] features)
            :base(features)
        {
            _handler = new Lazy<IHandlerFeature>(() => features.OfType<IHandlerFeature>().SingleOrDefault());
            _operationFeature = new Lazy<BatchOperationFeature>(() => features.OfType<BatchOperationFeature>().SingleOrDefault());
            _sharedMode = new Lazy<SharedFeature>(() => features.OfType<SharedFeature>().SingleOrDefault());
        }

        #region IConfigElement

        public override IConfigElementIdentity ElementIdentity
        {
            get
            {
                return _identity;
            }
        }

        public override OrdinaryConfigElementIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        #endregion

        public ITitleDescriptor TitleDescriptor
        {
            get
            {
                var feature = ElementFeatures.OfType<TitleFeature>().SingleOrDefault();
                return feature != null ? feature.TitleDescriptor : null;
            }
        }

        public IImageDescriptor ImageDescriptor
        {
            get
            {
                var feature = ElementFeatures.OfType<ImageFeature>().SingleOrDefault();
                return feature != null ? feature.ImageDescriptor : null;
            }
        }

        public IHandlerFeature Handler
        {
            get
            {
                return _handler.Value;
            }
        }

        public bool HasHandler 
        {
            get
            {
                return _handler.Value != null;
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

        public bool IsShared
        {
            get
            {
                return _sharedMode.Value != null;
            }
        }
    }
}
