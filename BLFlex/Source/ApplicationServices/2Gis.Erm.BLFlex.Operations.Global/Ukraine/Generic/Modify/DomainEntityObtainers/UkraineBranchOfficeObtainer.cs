using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineBranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>, IUkraineAdapted
    {
        private readonly IFinder _finder;

        public UkraineBranchOfficeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (UkraineBranchOfficeDomainEntityDto)domainEntityDto;

            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(dto.Id)) 
                ?? new BranchOffice { IsActive = true, Parts = new[] { new UkraineBranchOfficePart() } };

            BranchOfficeFlexSpecs.BranchOffices.Ukraine.Assign.Entity().Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}
