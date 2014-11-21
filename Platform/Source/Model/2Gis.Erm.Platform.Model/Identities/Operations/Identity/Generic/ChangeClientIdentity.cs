using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class ChangeClientIdentity : OperationIdentityBase<ChangeClientIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeClientIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "ChangeClient";
            }
        }
    }
}