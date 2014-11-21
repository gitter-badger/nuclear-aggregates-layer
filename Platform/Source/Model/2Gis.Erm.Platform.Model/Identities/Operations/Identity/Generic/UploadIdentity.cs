using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class UploadIdentity : OperationIdentityBase<UploadIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.UploadIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Upload";
            }
        }
    }
}