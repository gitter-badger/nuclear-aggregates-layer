using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.References
{
    public sealed class MetadataReference : MetadataElement<MetadataReference, MetadataReferenceBuilder>
    {
        private readonly Uri _referencedElementId;
        private IMetadataElementIdentity _identity;

        public MetadataReference(Uri referencedElementId)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            _identity = IdBuilder.StubUnique.AsIdentity();
            _referencedElementId = referencedElementId;
        }
        
        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public Uri ReferencedElementId
        {
            get
            {
                return _referencedElementId;
            }
        }

        public ICondition Condition { get; set; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}