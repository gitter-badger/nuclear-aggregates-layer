using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

using TimeZone = DoubleGis.Erm.Platform.Model.Entities.Security.TimeZone;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOrganizationUnitDtoService : GetDomainEntityDtoServiceBase<OrganizationUnit>
    {
        private readonly ISecureFinder _secureFinder;
        private readonly IFinder _finder;

        public GetOrganizationUnitDtoService(IUserContext userContext, ISecureFinder secureFinder, IFinder finder)
            : base(userContext)
        {
            _secureFinder = secureFinder;
            _finder = finder;
        }

        protected override IDomainEntityDto<OrganizationUnit> GetDto(long entityId)
        {
            var dto = _secureFinder.FindObsolete(new FindSpecification<OrganizationUnit>(x => x.Id == entityId))
                                   .Select(entity => new OrganizationUnitDomainEntityDto
                                                         {
                                                             Id = entity.Id,
                                                             Code = entity.Code,
                                                             Name = entity.Name,
                                                             DgppId = entity.DgppId,
                                                             CountryRef = new EntityReference { Id = entity.CountryId, Name = entity.Country.Name },
                                                             FirstEmitDate = entity.FirstEmitDate,
                                                             ErmLaunchDate = entity.ErmLaunchDate,
                                                             InfoRussiaLaunchDate = entity.InfoRussiaLaunchDate,
                                                             SyncCode1C = entity.SyncCode1C,
                                                             TimeZoneRef = new EntityReference { Id = entity.TimeZoneId, Name = null },
                                                             ElectronicMedia = entity.ElectronicMedia,
                                                             Timestamp = entity.Timestamp,
                                                             CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                             CreatedOn = entity.CreatedOn,
                                                             IsActive = entity.IsActive,
                                                             IsDeleted = entity.IsDeleted,
                                                             ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                             ModifiedOn = entity.ModifiedOn
                                                         })
                                   .Single();

            dto.TimeZoneRef.Name = _finder.Find(new FindSpecification<TimeZone>(tz => tz.Id == dto.TimeZoneRef.Id)).Map(q => q.Select(tz => tz.TimeZoneId)).Top();

            return dto;
        }

        protected override IDomainEntityDto<OrganizationUnit> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new OrganizationUnitDomainEntityDto
                       {
                           FirstEmitDate = DateTime.UtcNow,
                       };
        }
    }
}