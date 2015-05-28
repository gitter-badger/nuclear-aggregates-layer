using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify.DomainEntityObtainers
{
    public sealed class EmiratesBranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>, IEmiratesAdapted
    {
        private readonly IFinder _finder;

        public EmiratesBranchOfficeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (EmiratesBranchOfficeDomainEntityDto)domainEntityDto;

            var branchOffice = _finder.Find(Specs.Find.ById<BranchOffice>(dto.Id)).One() 
                ?? new BranchOffice { IsActive = true };

            BranchOfficeFlexSpecs.BranchOffices.Emirates.Assign.Entity().Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}