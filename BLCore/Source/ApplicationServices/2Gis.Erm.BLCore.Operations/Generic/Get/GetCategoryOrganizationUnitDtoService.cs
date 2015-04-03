using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    // FIXME {v.lapeev, 04.09.2013}: поддержка интеграции с ЛК, при запиливании полноценного сервиса подсчета стоимости заказов (он же фасад для ЛК) - выпилить
    public sealed class GetCategoryOrganizationUnitDtoService : GetDomainEntityDtoServiceBase<CategoryOrganizationUnit>
    {
        private readonly ISecureFinder _finder;

        public GetCategoryOrganizationUnitDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<CategoryOrganizationUnit> GetDto(long entityId)
        {
            return _finder.Find<CategoryOrganizationUnit>(x => x.Id == entityId)
                          .Select(entity => new CategoryOrganizationUnitDomainEntityDto
                          {
                              Id = entity.Id,
                              CategoryRef = new EntityReference { Id = entity.CategoryId, Name = entity.Category.Name },
                              CategoryGroupRef = new EntityReference { Id = entity.CategoryGroupId, Name = entity.CategoryGroup.CategoryGroupName },
                              OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                              OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
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

        protected override IDomainEntityDto<CategoryOrganizationUnit> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {   
            // пока отключаем возможность создание, т.к. потребитель этого кода - ЛК должен использовать только в режиме readonly
            throw new System.NotSupportedException();
        }
    }
}
