using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    [DataContract]
    public sealed class StartSimplifiedReleaseIdentity : OperationIdentityBase<StartSimplifiedReleaseIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.StartSimplifiedReleaseIdentity; }
        }

        public override string Description
        {
            get { return "Start simplified release process for projects that is not migrated to ERM yet"; }
        }
    }
}
