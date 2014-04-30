using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.References
{
    public sealed class MetadataReferenceBuilder : MetadataElementBuilder<MetadataReferenceBuilder, MetadataReference>
    {
        private ICondition _condition;
        private Uri _referenceElementId;

        public MetadataReferenceBuilder For(Uri referenceElementId)
        {
            _referenceElementId = referenceElementId;
            return this;
        }

        public MetadataReferenceBuilder ApplyCondition(ICondition condition)
        {
            _condition = condition;
            return this;
        }

        protected override MetadataReference Create()
        {
            return new MetadataReference(_referenceElementId) { Condition = _condition };
        }
    }
}