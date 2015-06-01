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
    public class GetDepartmentDtoService : GetDomainEntityDtoServiceBase<Department>
    {
        private readonly IFinder _finder;

        public GetDepartmentDtoService(IUserContext userContext, IFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Department> GetDto(long entityId)
        {
            return _finder.FindObsolete(new FindSpecification<Department>(x => x.Id == entityId))
                          .Select(entity => new DepartmentDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    Name = entity.Name,
                                                    ParentRef = new EntityReference { Id = entity.ParentId, Name = entity.ParentId.HasValue ? entity.Parent.Name : string.Empty },
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                    CreatedOn = entity.CreatedOn,
                                                    IsActive = entity.IsActive,
                                                    IsDeleted = entity.IsDeleted,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                    ModifiedOn = entity.ModifiedOn,
                                                    Timestamp = entity.Timestamp
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<Department> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new DepartmentDomainEntityDto();
        }
    }
}