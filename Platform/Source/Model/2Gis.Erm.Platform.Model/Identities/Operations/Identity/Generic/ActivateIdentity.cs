using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class ActivateIdentity : OperationIdentityBase<ActivateIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ActivateIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Activate";
            }
        }
    }
}