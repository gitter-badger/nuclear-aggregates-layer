namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    public class BulkUpdateIdentity : OperationIdentityBase<BulkUpdateIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BulkUpdateIdentity; }
        }

        public override string Description
        {
            get { return "Bulk update"; }
        }
    }
}