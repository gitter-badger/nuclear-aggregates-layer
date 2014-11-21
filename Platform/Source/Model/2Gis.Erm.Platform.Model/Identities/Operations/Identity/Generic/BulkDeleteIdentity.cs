using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class BulkDeleteIdentity : OperationIdentityBase<BulkDeleteIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BulkDeleteIdentity; }
        }

        public override string Description
        {
            get { return "Bulk delete"; }
        }
    }
}
