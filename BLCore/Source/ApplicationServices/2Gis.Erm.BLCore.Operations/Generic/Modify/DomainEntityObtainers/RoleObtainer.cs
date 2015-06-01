using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class RoleObtainer : IBusinessModelEntityObtainer<Role>, IAggregateReadModel<Role>
    {
        private readonly IFinder _finder;

        public RoleObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Role ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (RoleDomainEntityDto)domainEntityDto;

            var role = _finder.Find(Specs.Find.ById<Role>(dto.Id)).One() ??
                       new Role { Id = dto.Id };

            if (dto.Timestamp == null && role.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            role.Name = dto.Name;
            role.Timestamp = dto.Timestamp;

            return role;
        }
    }
}