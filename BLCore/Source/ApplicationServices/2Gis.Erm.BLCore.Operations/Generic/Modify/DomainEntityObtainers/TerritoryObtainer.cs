using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class TerritoryObtainer : IBusinessModelEntityObtainer<Territory>, IAggregateReadModel<User>
    {
        private readonly IFinder _finder;

        public TerritoryObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Territory ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (TerritoryDomainEntityDto)domainEntityDto;

            var territory = _finder.Find(Specs.Find.ById<Territory>(dto.Id)).SingleOrDefault() ??
                            new Territory { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && territory.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            territory.Name = dto.Name;
            territory.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
            territory.Timestamp = dto.Timestamp;

            return territory;
        }
    }
}