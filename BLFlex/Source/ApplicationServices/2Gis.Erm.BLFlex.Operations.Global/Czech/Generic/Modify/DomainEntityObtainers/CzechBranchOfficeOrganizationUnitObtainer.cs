using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.DomainEntityObtainers
{
    public class CzechBranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>, ICzechAdapted
    {
        private readonly IFinder _finder;

        public CzechBranchOfficeOrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CzechBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id)) 
                ?? new BranchOfficeOrganizationUnit { IsActive = true };

            BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Czech.Assign.Entity().Assign(dto, branchOfficeOrganizationUnit);

            return branchOfficeOrganizationUnit;
        }
    }
}