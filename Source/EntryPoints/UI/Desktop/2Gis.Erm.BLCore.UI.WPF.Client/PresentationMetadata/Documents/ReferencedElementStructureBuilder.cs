using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class ReferencedElementStructureBuilder : ViewModelStructuresBuilder<ReferencedElementStructureBuilder, ReferencedElementStructure, OrdinaryConfigElementIdentity>
    {
        private ICondition _condition;
        private IConfigElementIdentity _referenceIdentity;

        public ReferencedElementStructureBuilder For(IConfigElementIdentity referenceIdentity)
        {
            _referenceIdentity = referenceIdentity;
            return this;
        }

        public ReferencedElementStructureBuilder ApplyCondition(ICondition condition)
        {
            _condition = condition;
            return this;
        }

        protected override ReferencedElementStructure Create()
        {
            return new ReferencedElementStructure(_referenceIdentity, Features) { Condition = _condition };
        }
    }
}