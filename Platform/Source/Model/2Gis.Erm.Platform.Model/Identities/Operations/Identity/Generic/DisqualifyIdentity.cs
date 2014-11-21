using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class DisqualifyIdentity : OperationIdentityBase<DisqualifyIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DisqualifyIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Disqualify";
            }
        }
    }
}