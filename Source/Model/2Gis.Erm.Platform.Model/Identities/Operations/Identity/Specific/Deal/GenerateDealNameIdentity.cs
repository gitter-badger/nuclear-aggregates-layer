using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal
{
    // 2+ \Platform\Source\Model\2Gis.Erm.Platform.Model\Identities\Operations\Identity\Specific\Deal
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
