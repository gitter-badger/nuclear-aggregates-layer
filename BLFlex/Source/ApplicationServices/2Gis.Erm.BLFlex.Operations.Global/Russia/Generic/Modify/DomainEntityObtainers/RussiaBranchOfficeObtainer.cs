using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

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
            var branchOffice = _finder.Find(Specs.Find.ById<BranchOffice>(dto.Id)).One()
                ?? new BranchOffice { IsActive = true };

            BranchOfficeFlexSpecs.BranchOffices.Russia.Assign.Entity().Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}
