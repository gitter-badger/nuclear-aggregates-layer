using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify.DomainEntityObtainers
{
    public class KazakhstanBranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly IAssignSpecification<KazakhstanBranchOfficeDomainEntityDto, BranchOffice> _assignSpecification;

        public KazakhstanBranchOfficeObtainer(IFinder finder)
        {
            _finder = finder;
            _assignSpecification = BranchOfficeFlexSpecs.BranchOffices.Kazakhstan.Assign.Entity();
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (KazakhstanBranchOfficeDomainEntityDto)domainEntityDto;

            var branchOffice = _finder.Find(Specs.Find.ById<BranchOffice>(dto.Id)).One()
                               ?? new BranchOffice { IsActive = true };

            _assignSpecification.Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}
