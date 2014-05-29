using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify
{
    public abstract class DynamicBranchOfficeObtainerBase<TDto, TPart> : BranchOfficeObtainerBase<TDto, BranchOffice>
        where TDto : IDomainEntityDto<BranchOffice> 
        where TPart : IEntityPart, new()
    {
        protected DynamicBranchOfficeObtainerBase(IFinder finder) 
            : base(finder)
        {
        }

        protected override BranchOffice CreateEntity()
        {
            return new BranchOffice { IsActive = true, Parts = new IEntityPart[] { new TPart() } };
        }
    }
}