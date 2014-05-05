using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class AttachedMetadata :
        ViewModelMetadata<AttachedMetadata, AttachedMetadataBuilder>,
        IDocumentElementMetadata,
        IHandlerBoundElement
    {
        private readonly Lazy<IHandlerFeature> _handler;
        private IMetadataElementIdentity _identity;

        public AttachedMetadata(IMetadataElementIdentity metadataElementIdentity, IEnumerable<IMetadataFeature> features) 
            : base(features)
        {
            _identity = metadataElementIdentity;
            _handler = new Lazy<IHandlerFeature>(() => Features.OfType<IHandlerFeature>().SingleOrDefault());
        }
        
        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
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
        
        public ICondition Condition { get; set; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}