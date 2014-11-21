using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class DownloadIdentity : OperationIdentityBase<DownloadIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DownloadIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Download";
            }
        }
    }
}