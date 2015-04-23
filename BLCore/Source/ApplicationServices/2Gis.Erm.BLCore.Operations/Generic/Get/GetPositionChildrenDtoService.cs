using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPositionChildrenDtoService : GetDomainEntityDtoServiceBase<PositionChildren>
    {
        private readonly ISecureFinder _finder;

        public GetPositionChildrenDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<PositionChildren> GetDto(long entityId)
        {
            return _finder.Find<PositionChildren>(x => x.Id == entityId)
                          .Select(entity => new PositionChildrenDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    MasterPositionRef = new EntityReference { Id = entity.MasterPositionId, Name = entity.MasterPosition.Name },
                                                    ChildPositionRef = new EntityReference { Id = entity.ChildPositionId, Name = (entity.ChildPosition == null) ? string.Empty : entity.ChildPosition.Name },
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

        protected override IDomainEntityDto<PositionChildren> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new PositionChildrenDomainEntityDto();

            switch (parentEntityName)
            {
                case EntityName.Position:
                    {
                        dto.MasterPositionRef = new EntityReference()
                            {
                                Id = parentEntityId.Value,
                                Name = _finder.Find<Position>(x => x.Id == parentEntityId).Select(x => x.Name).SingleOrDefault()
                            };

                    }

                    break;
            }

            return dto;
        }
    }
}