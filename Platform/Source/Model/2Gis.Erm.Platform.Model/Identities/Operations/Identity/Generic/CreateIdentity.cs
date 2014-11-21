using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class CreateIdentity : OperationIdentityBase<CreateIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CreateIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Create";
            }
        }
    }
}