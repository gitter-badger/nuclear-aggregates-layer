using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PlatformObtainer : ISimplifiedModelEntityObtainer<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>
    {
        private readonly IFinder _finder;

        public PlatformObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public DoubleGis.Erm.Platform.Model.Entities.Erm.Platform ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PlatformDomainEntityDto)domainEntityDto;

            var platform = _finder.Find(PlatformSpecifications.Find.ById(dto.Id)).SingleOrDefault() ??
                           new DoubleGis.Erm.Platform.Model.Entities.Erm.Platform { Id = dto.Id };

            if (dto.Timestamp == null && platform.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            platform.Name = dto.Name;
            platform.MinPlacementPeriodEnum = (int)dto.MinPlacementPeriodEnum;
            platform.PlacementPeriodEnum = (int)dto.PlacementPeriodEnum;
            platform.DgppId = dto.DgppId;
            platform.IsSupportedByExport = dto.IsSupportedByExport;
            platform.Timestamp = dto.Timestamp;

            return platform;
        }
    }
}