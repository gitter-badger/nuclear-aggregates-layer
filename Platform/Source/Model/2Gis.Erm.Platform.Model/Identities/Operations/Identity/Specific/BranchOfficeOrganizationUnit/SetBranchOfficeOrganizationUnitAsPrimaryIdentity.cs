using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit
{
    public class SetBranchOfficeOrganizationUnitAsPrimaryIdentity : OperationIdentityBase<SetBranchOfficeOrganizationUnitAsPrimaryIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SetBranchOfficeOrganizationUnitAsPrimaryIdentity; }
        }

        public override string Description
        {
            get { return "Сделать основным"; }
        }
    }
}