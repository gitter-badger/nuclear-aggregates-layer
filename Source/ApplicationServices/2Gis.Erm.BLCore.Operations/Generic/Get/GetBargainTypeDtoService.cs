using System.Linq;

using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetBargainTypeDtoService : GetDomainEntityDtoServiceBase<BargainType>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetBargainTypeDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new BargainTypeDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            return _finder.Find<BargainType>(x => x.Id == entityId)
                          .Select(entity => new BargainTypeDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  SyncCode1C = entity.SyncCode1C,
                                  VatRate = entity.VatRate,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();
        }
    }
}