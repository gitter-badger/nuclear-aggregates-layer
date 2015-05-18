using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit
{
    public class ReopenLimitIdenitity : OperationIdentityBase<ReopenLimitIdenitity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ReopenLimitIdentity; }
        }

        public override string Description
        {
            get { return "Переоткрытие лимита"; }
        }
    }
}