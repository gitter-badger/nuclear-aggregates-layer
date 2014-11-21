using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class AssignIdentity : OperationIdentityBase<AssignIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.AssignIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Assign";
            }
        }
    }
}