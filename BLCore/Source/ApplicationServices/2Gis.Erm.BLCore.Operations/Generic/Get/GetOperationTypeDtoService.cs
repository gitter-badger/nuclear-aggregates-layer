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
    public class GetOperationTypeDtoService : GetDomainEntityDtoServiceBase<OperationType>
    {
        private readonly ISecureFinder _finder;

        public GetOperationTypeDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<OperationType> GetDto(long entityId)
        {
            return _finder.Find<OperationType>(x => x.Id == entityId)
                          .Select(entity => new OperationTypeDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    Name = entity.Name,
                                                    Description = entity.Description,
                                                    IsPlus = entity.IsPlus,
                                                    IsInSyncWith1C = entity.IsInSyncWith1C,
                                                    SyncCode1C = entity.SyncCode1C,
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

        protected override IDomainEntityDto<OperationType> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new OperationTypeDomainEntityDto
                       {
                           IsActive = true,
                       };
        }
    }
}