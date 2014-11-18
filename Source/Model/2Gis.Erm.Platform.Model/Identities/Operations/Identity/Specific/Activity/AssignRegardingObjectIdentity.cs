namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity
{
	public sealed class AssignRegardingObjectIdentity : OperationIdentityBase<AssignRegardingObjectIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
				return OperationIdentityIds.AssignRegardingObjectIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Assigns regarding object to an activity.";
            }
        }
    }
}