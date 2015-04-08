using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel
{
    [DataContract]
    public class CancelIdentity : OperationIdentityBase<CancelIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CancelIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Cancel";
            }
        }
    }    
}
