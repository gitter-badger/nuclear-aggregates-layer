﻿using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetDepartmentDtoSerivce : GetDomainEntityDtoServiceBase<Department>
    {
        private readonly IFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetDepartmentDtoSerivce(IUserContext userContext, IFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<Department> GetDto(long entityId)
        {
            return _finder.Find<Department>(x => x.Id == entityId)
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

        protected override IDomainEntityDto<Department> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new DepartmentDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}