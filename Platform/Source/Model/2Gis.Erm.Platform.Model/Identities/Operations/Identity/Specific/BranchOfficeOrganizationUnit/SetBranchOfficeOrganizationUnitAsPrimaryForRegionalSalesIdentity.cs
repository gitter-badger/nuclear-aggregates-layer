using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit
{
    public class SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity : OperationIdentityBase<SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity>,
                                                                                    INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity; }
        }

        public override string Description
        {
            get { return "Сделать основным для региональных продаж"; }

        }
    }
}