using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify
{
    public abstract class DynamicBranchOfficeOrganizationUnitObtainerBase<TDto, TPart> : BranchOfficeObtainerBase<TDto, BranchOfficeOrganizationUnit>
        where TDto : IDomainEntityDto<BranchOfficeOrganizationUnit>
        where TPart : IEntityPart, new()
    {
        protected DynamicBranchOfficeOrganizationUnitObtainerBase(IFinder finder)
            : base(finder)
        {
        }

        protected override BranchOfficeOrganizationUnit CreateEntity()
        {
            return new BranchOfficeOrganizationUnit { IsActive = true, Parts = new IEntityPart[] { new TPart() } };
        }
    }
}