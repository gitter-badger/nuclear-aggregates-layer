using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete
{  
    [DataContract]
    public class CompleteIdentity : OperationIdentityBase<CompleteIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CompleteIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Complete";
            }
        }
    }    
}
