using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class AttachedMetadataBuilder : ViewModelMetadataBuilder<AttachedMetadataBuilder, AttachedMetadata>
    {
        private readonly HandlerFeatureAspect<AttachedMetadataBuilder, AttachedMetadata> _handler;
        private ICondition _condition;

        public AttachedMetadataBuilder()
        {
            _handler = new HandlerFeatureAspect<AttachedMetadataBuilder, AttachedMetadata>(this);
        }

        public HandlerFeatureAspect<AttachedMetadataBuilder, AttachedMetadata> Handler
        {
            get { return _handler; }
        }

        public AttachedMetadataBuilder ApplyCondition(ICondition condition)
        {
            _condition = condition;
            return this;
        }

        protected override AttachedMetadata Create()
        {
            return new AttachedMetadata(IdBuilder.StubUnique.AsIdentity(), Features) { Condition = _condition };
        }
    }


}