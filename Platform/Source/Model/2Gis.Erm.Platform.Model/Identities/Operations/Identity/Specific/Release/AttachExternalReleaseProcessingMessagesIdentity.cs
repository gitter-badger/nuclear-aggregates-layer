using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    public sealed class AttachExternalReleaseProcessingMessagesIdentity : OperationIdentityBase<AttachExternalReleaseProcessingMessagesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.AttachExternalReleaseProcessingMessagesIdentity; }
        }

        public override string Description
        {
            get { return "Finish previously started release process."; }
        }
    }
}