using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class BulkCreateIdentity : OperationIdentityBase<BulkCreateIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BulkCreateIdentity; }
        }

        public override string Description
        {
            get { return "Bulk create"; }
        }
    }
}
