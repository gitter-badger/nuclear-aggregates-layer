using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    [DataContract]
    public sealed class RevertReleaseIdentity : OperationIdentityBase<RevertReleaseIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.RevertReleaseIdentity; }
        }

        public override string Description
        {
            get { return "Revert previously successfully finished release"; }
        }
    }
}
