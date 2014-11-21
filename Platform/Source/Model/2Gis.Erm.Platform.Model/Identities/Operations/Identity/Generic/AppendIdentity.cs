using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class AppendIdentity : OperationIdentityBase<AppendIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.AppendIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Append";
            }
        }
    }
}