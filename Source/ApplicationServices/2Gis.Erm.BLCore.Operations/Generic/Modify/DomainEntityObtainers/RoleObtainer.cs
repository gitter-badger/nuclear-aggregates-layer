﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Roles;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

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

            var role = _finder.Find(RoleSpecifications.Find.ById(dto.Id)).SingleOrDefault() ??
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