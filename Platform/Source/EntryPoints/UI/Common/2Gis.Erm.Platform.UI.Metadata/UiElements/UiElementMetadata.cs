using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements
{
    public sealed class UIElementMetadata : MetadataElement<UIElementMetadata, UIElementMetadataBuilder>,
        INamedElement,
        ITitledElement,
        IImageBoundElement,
        IHandlerBoundElement,
        IOperationsBoundElement
    {
        private IMetadataElementIdentity _identity;

        public UIElementMetadata(Uri id, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = id.AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public IStringResourceDescriptor NameDescriptor
        {
            get
            {
                var feature = Features.OfType<NameFeature>().SingleOrDefault();
                return feature != null ? feature.NameDescriptor : null;
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

        public IImageDescriptor ImageDescriptor
        {
            get
            {
                var feature = Features.OfType<ImageFeature>().SingleOrDefault();
                return feature != null ? feature.ImageDescriptor : null;
            }
        }

        public IHandlerFeature Handler
        {
            get
            {
                return Features.OfType<IHandlerFeature>().SingleOrDefault();
            }
        }

        public bool HasOperations
        {
            get
            {
                var operationsSet = Features.OfType<OperationsSetFeature>().SingleOrDefault();
                return operationsSet != null && operationsSet.OperationFeatures.Any();
            }
        }

        public IEnumerable<OperationFeature> OperationFeatures
        {
            get
            {
                var operationsSet = Features.OfType<OperationsSetFeature>().SingleOrDefault();
                return operationsSet != null ? operationsSet.OperationFeatures : Enumerable.Empty<OperationFeature>();
            }
        }

        public bool HasHandler
        {
            get
            {
                return Features.OfType<IHandlerFeature>().SingleOrDefault() != null;
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}
