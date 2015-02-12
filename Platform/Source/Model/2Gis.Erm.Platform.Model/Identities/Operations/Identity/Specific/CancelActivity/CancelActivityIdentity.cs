using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.CancelActivity
{
    [DataContract]
    public class CancelActivityIdentity : OperationIdentityBase<CancelActivityIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CancelActivityIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "CancelActivity";
            }
        }
    }    
}
