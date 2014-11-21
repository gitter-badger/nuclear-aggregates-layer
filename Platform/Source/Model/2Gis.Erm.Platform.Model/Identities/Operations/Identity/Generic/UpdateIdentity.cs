using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class UpdateIdentity : OperationIdentityBase<UpdateIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.UpdateIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Update";
            }
        }
    }
}