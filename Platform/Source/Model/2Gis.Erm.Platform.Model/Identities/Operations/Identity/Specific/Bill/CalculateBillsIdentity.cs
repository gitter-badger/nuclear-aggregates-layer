using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bill
{
    [DataContract]
    public sealed class CalculateBillsIdentity : OperationIdentityBase<CalculateBillsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CalculateBillsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Рассчитать суммы счетов";
            }
        }
    }
}