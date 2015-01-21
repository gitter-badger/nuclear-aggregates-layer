using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class CreateBlankFirmsIdentity : OperationIdentityBase<CreateBlankFirmsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CreateBlankFirmsIdentity; }
        }

        public override string Description
        {
            get { return "Операция создания пустых фирм"; }
        }
    }
}