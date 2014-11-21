using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    [DataContract]
    public sealed class StartReleaseIdentity : OperationIdentityBase<StartReleaseIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.StartReleaseIdentity; }
        }

        public override string Description
        {
            get { return "Start release process"; }
        }
    }
}
