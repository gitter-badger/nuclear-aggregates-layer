using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PlatformObtainer : ISimplifiedModelEntityObtainer<Platform.Model.Entities.Erm.Platform>
    {
        private readonly IFinder _finder;

        public PlatformObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Platform.Model.Entities.Erm.Platform ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PlatformDomainEntityDto)domainEntityDto;

            var platform = _finder.Find(Specs.Find.ById<Platform.Model.Entities.Erm.Platform>(dto.Id)).SingleOrDefault() ??
                           new Platform.Model.Entities.Erm.Platform { Id = dto.Id };

            if (dto.Timestamp == null && platform.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            platform.Name = dto.Name;
            platform.MinPlacementPeriodEnum = dto.MinPlacementPeriodEnum;
            platform.PlacementPeriodEnum = dto.PlacementPeriodEnum;
            platform.DgppId = dto.DgppId;
            platform.IsSupportedByExport = dto.IsSupportedByExport;
            platform.Timestamp = dto.Timestamp;

            return platform;
        }
    }
}