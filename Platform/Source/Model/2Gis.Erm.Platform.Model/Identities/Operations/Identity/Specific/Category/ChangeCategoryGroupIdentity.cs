using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Category
{
    public class ChangeCategoryGroupIdentity : OperationIdentityBase<ChangeCategoryGroupIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ChangeCategoryGroupIdentity; }
        }

        public override string Description
        {
            get { return "Смена коэффициента рубрики в контексте отделения организации"; }
        }
    }
}