using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal
{
    [DataContract]
    public sealed class GenerateDealNameIdentity : OperationIdentityBase<GenerateDealNameIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.GenerateDealNameIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "GenereateDealName";
            }
        }
    }
}
