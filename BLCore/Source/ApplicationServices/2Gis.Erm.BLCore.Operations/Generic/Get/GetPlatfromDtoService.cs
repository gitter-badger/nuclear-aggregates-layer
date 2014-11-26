using System.Linq;

using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPlatfromDtoService : GetDomainEntityDtoServiceBase<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetPlatfromDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform> GetDto(long entityId)
        {
            return _finder.Find<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>(x => x.Id == entityId)
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

        protected override IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new PlatformDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}