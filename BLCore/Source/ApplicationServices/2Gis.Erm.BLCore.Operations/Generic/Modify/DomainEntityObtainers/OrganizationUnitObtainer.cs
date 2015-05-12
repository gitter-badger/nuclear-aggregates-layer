using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class OrganizationUnitObtainer : IBusinessModelEntityObtainer<OrganizationUnit>, IAggregateReadModel<User>
    {
        private readonly IFinder _finder;

        public OrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public OrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrganizationUnitDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<OrganizationUnit>(dto.Id)).SingleOrDefault() ??
                         new OrganizationUnit { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.Name = dto.Name;
            entity.Code = dto.Code;
            entity.ElectronicMedia = dto.ElectronicMedia;
            entity.CountryId = dto.CountryRef.Id.Value;
            entity.FirstEmitDate = dto.FirstEmitDate;
            entity.ErmLaunchDate = dto.ErmLaunchDate;
            entity.InfoRussiaLaunchDate = dto.InfoRussiaLaunchDate;
            entity.SyncCode1C = dto.SyncCode1C;
            entity.TimeZoneId = dto.TimeZoneRef.Id.Value;
            entity.DgppId = dto.DgppId;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}