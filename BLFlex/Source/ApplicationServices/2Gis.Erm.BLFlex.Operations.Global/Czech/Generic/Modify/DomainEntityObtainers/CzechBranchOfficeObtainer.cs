using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.DomainEntityObtainers
{
    public class CzechBranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>, ICzechAdapted
    {
        private readonly IFinder _finder;

        public CzechBranchOfficeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CzechBranchOfficeDomainEntityDto)domainEntityDto;
            var branchOffice = _finder.Find(Specs.Find.ById<BranchOffice>(dto.Id)).One() 
                ?? new BranchOffice { IsActive = true };

            BranchOfficeFlexSpecs.BranchOffices.Czech.Assign.Entity().Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}