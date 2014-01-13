using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class AttachedElementStructureBuilder : ViewModelStructuresBuilder<AttachedElementStructureBuilder, AttachedElementStructure, OrdinaryConfigElementIdentity>
    {
        private ICondition _condition;
        private readonly HandlerFeatureAspect<AttachedElementStructureBuilder, AttachedElementStructure> _handler;

        public AttachedElementStructureBuilder()
        {
            _handler = new HandlerFeatureAspect<AttachedElementStructureBuilder, AttachedElementStructure>(this);
        }

        public HandlerFeatureAspect<AttachedElementStructureBuilder, AttachedElementStructure> Handler
        {
            get { return _handler; }
        }

        public AttachedElementStructureBuilder ApplyCondition(ICondition condition)
        {
            _condition = condition;
            return this;
        }

        protected override AttachedElementStructure Create()
        {
            return new AttachedElementStructure(Features) { Condition = _condition };
        }
    }


}