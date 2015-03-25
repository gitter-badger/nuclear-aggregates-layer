using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.Elements.Aspects.Conditions;
using NuClear.Metamodeling.Elements.Identities.Builder;

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
            return new AttachedMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Stub().Unique().Build().AsIdentity(), Features) { Condition = _condition };
        }
    }


}