using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class DeleteIdentity : OperationIdentityBase<DeleteIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DeleteIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Delete";
            }
        }
    }
}