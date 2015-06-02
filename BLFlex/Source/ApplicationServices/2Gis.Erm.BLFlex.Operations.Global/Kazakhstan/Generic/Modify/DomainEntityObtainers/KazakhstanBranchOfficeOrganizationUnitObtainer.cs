using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify.DomainEntityObtainers
{
    public class KazakhstanBranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;

        public KazakhstanBranchOfficeOrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (KazakhstanBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id))
                ?? new BranchOfficeOrganizationUnit { IsActive = true };

            BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Kazakhstan.Assign.Entity().Assign(dto, branchOfficeOrganizationUnit);

            return branchOfficeOrganizationUnit;
        }
    }
}