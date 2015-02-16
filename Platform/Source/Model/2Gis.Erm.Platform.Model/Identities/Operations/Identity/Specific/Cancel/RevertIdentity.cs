using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel
{  
    [DataContract]
    public class RevertIdentity : OperationIdentityBase<RevertIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.RevertIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Revert";
            }
        }
    }    
}
