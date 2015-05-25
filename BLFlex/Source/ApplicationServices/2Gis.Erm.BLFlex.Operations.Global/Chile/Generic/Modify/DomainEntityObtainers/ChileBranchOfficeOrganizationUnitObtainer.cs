using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileBranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>, IChileAdapted
    {
        private readonly IFinder _finder;

        public ChileBranchOfficeOrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ChileBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id))
                ?? new BranchOfficeOrganizationUnit { IsActive = true, Parts = new[] { new ChileBranchOfficeOrganizationUnitPart() } };

            BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Chile.Assign.Entity().Assign(dto, branchOfficeOrganizationUnit);

            return branchOfficeOrganizationUnit;
        }
    }
}
