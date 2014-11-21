using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class BulkDeactivateIdentity : OperationIdentityBase<BulkDeactivateIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BulkDeactivateIdentity; }
        }

        public override string Description
        {
            get { return "Bulk deactivate"; }
        }
    }
}
