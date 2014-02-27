using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class ContributionTypeObtainer : ISimplifiedModelEntityObtainer<ContributionType>
    {
        private readonly IFinder _finder;

        public ContributionTypeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public ContributionType ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ContributionTypeDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(BranchOfficeSpecifications.Find.ContributionTypeById(dto.Id)).SingleOrDefault() ??
                         new ContributionType { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.Name = dto.Name;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}