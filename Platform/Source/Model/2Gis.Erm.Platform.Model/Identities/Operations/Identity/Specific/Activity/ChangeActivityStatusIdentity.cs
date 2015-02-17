using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity
{
     [DataContract]
    public class ChangeActivityStatusIdentity : OperationIdentityBase<ChangeActivityStatusIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeActivityStatusIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "ChangeActivityStatus";
            }
        }
    }
}
