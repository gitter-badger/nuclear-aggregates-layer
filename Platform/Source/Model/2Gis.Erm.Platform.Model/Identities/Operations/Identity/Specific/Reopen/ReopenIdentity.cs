using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen
{  
    [DataContract]
    public class ReopenIdentity : OperationIdentityBase<ReopenIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ReopenIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Reopen";
            }
        }
    }    
}
