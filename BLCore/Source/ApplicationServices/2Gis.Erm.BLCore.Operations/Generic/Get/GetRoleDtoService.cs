using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetRoleDtoService : GetDomainEntityDtoServiceBase<Role>
    {
        private readonly IFinder _finder;

        public GetRoleDtoService(IUserContext userContext, IFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Role> GetDto(long entityId)
        {
            return _finder.Find<Role>(x => x.Id == entityId)
                          .Select(entity => new RoleDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    Name = entity.Name,
                                                    Timestamp = entity.Timestamp,
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                                    CreatedOn = entity.CreatedOn,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                                    ModifiedOn = entity.ModifiedOn
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<Role> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new RoleDomainEntityDto();
        }
    }
}