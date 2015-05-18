using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge
{
    public class DeleteChargesForPeriodAndProjectIdentity : OperationIdentityBase<DeleteChargesForPeriodAndProjectIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.DeleteChargesForPeriodAndProjectIdentity; }
        }

        public override string Description
        {
            get { return "Удаление всех Charges по проекту за заданный период"; }
        }
    }
}