using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client
{
    public class SetMainFirmIdentity : OperationIdentityBase<SetMainFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SetMainFirmIdentity; }
        }

        public override string Description
        {
            get { return "Смена главной фирмы у клиента"; }
        }
    }
}