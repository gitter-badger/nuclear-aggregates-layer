using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    [DataContract]
    public sealed class EnsureOrdersForReleaseCompletelyExportedIdentity : OperationIdentityBase<EnsureOrdersForReleaseCompletelyExportedIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.EnsureOrdersForReleaseCompletelyExportedIdentity; }
        }

        public override string Description
        {
            get { return "Ensure that all orders involved to releasing process is exported to target storages"; }
        }
    }
}