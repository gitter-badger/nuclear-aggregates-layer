using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    [DataContract]
    public sealed class FinishReleaseIdentity : OperationIdentityBase<FinishReleaseIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.FinishReleaseIdentity; }
        }

        public override string Description
        {
            get { return "Finish previosly started release process."; }
        }
    }
}
