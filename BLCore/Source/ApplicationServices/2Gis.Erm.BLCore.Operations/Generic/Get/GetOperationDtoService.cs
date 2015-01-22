using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOperationDtoService : GetDomainEntityDtoServiceBase<Operation>
    {
        private readonly ISecureFinder _finder;

        public GetOperationDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Operation> GetDto(long entityId)
        {
            return _finder.Find<Operation>(x => x.Id == entityId)
                          .Select(x => new OperationDomainEntityDto()
                              {
                                  Id = x.Id,
                                  Description = x.Description,
                                  StartTime = x.StartTime,
                                  FinishTime = x.FinishTime,
                                  Status = x.Status,
                                  Type = x.Type,
                                  OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = x.OrganizationUnit.Name },
                                  OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn,
                                  Timestamp = x.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Operation> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            throw new NotSupportedException();
        }
    }
}