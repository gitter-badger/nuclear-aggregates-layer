using System;
using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetProjectDtoService : GetDomainEntityDtoServiceBase<Project>
    {
        private readonly ISecureFinder _finder;

        public GetProjectDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Project> GetDto(long entityId)
        {
            return _finder.Find<Project>(x => x.Id == entityId)
                          .Select(x => new ProjectDomainEntityDto
                              {
                                  Id = x.Id,
                                  DisplayName = x.DisplayName,
                                  NameLat = x.NameLat,
                                  IsActive = x.IsActive,
                                  DefaultLang = x.DefaultLang,
                                  OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = x.OrganizationUnit.Name },
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn,
                                  Timestamp = x.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Project> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            throw new NotSupportedException();
        }
    }
}