using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific
{
    [DataContract]
    public sealed class PerformedOperationProcessingAnalysisIdentity : OperationIdentityBase<PerformedOperationProcessingAnalysisIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.PerformedOperationProcessingAnalysisIdentity; }
        }

        public override string Description
        {
            get { return "Специальная identity для операций генерируемых при нагрузочном тестировании и т.п. тестовых целях"; }
        }
    }
}
