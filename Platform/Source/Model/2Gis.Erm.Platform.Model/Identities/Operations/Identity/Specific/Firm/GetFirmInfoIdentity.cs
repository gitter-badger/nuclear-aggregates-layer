using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class GetFirmInfoIdentity : OperationIdentityBase<GetFirmInfoIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetFirmInfoIdentity; }
        }

        public override string Description
        {
            get { return "Получение данных по фирме для iWeb"; }
        }
    }
}