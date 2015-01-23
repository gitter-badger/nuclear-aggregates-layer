using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public class RussiaBranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>, IRussiaAdapted
    {
        private readonly IFinder _finder;

        public RussiaBranchOfficeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (RussiaBranchOfficeDomainEntityDto)domainEntityDto;
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(dto.Id)) 
                ?? new BranchOffice { IsActive = true };

            BranchOfficeFlexSpecs.BranchOffices.Russia.Assign.Entity().Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}
