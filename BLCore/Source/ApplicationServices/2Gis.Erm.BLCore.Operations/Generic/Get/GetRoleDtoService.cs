using System.Linq;

using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

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
            return _finder.FindObsolete(new FindSpecification<Role>(x => x.Id == entityId))
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

        protected override IDomainEntityDto<Role> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new RoleDomainEntityDto();
        }
    }
}