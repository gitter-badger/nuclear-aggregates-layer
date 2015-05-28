﻿using System.Linq;

using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetUserDtoService : GetDomainEntityDtoServiceBase<User>
    {
        private readonly IFinder _finder;

        public GetUserDtoService(IUserContext userContext, IFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<User> GetDto(long entityId)
        {
            return _finder.FindObsolete(new FindSpecification<User>(x => x.Id == entityId))
                          .Select(entity => new UserDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    FirstName = entity.FirstName,
                                                    LastName = entity.LastName,
                                                    DisplayName = entity.DisplayName,
                                                    Account = entity.Account,
                                                    DepartmentRef = new EntityReference { Id = entity.DepartmentId, Name = entity.Department.Name },
                                                    ParentRef = new EntityReference { Id = entity.ParentId, Name = entity.Parent.DisplayName },
                                                    IsServiceUser = entity.IsServiceUser,
                                                    Timestamp = entity.Timestamp,
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                    CreatedOn = entity.CreatedOn,
                                                    IsActive = entity.IsActive,
                                                    IsDeleted = entity.IsDeleted,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                    ModifiedOn = entity.ModifiedOn
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<User> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new UserDomainEntityDto();
        }
    }
}