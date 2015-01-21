using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public class ExportAccountDetailsTo1CIdentity : OperationIdentityBase<ExportAccountDetailsTo1CIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ExportAccountDetailsTo1CIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Выгрузка лицевых счетов в 1С";
            }
        }
    }
}