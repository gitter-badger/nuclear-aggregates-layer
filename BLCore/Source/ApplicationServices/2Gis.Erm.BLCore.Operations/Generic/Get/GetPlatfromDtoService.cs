using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPlatfromDtoService : GetDomainEntityDtoServiceBase<Platform.Model.Entities.Erm.Platform>
    {
        private readonly ISecureFinder _finder;

        public GetPlatfromDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Platform.Model.Entities.Erm.Platform> GetDto(long entityId)
        {
            return _finder.Find<Platform.Model.Entities.Erm.Platform>(x => x.Id == entityId)
                          .Select(entity => new PlatformDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  DgppId = entity.DgppId,
                                  MinPlacementPeriodEnum = entity.MinPlacementPeriodEnum,
                                  PlacementPeriodEnum = entity.PlacementPeriodEnum,
                                  IsSupportedByExport = entity.IsSupportedByExport,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                  CreatedOn = entity.CreatedOn,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Platform.Model.Entities.Erm.Platform> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new PlatformDomainEntityDto();
        }
    }
}