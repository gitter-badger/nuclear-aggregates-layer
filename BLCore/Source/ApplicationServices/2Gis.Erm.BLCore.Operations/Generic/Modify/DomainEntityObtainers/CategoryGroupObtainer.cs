﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class CategoryGroupObtainer : ISimplifiedModelEntityObtainer<CategoryGroup>
    {
        private readonly IFinder _finder;

        public CategoryGroupObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public CategoryGroup ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CategoryGroupDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<CategoryGroup>(dto.Id)).SingleOrDefault() ??
                         new CategoryGroup { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.CategoryGroupName = dto.CategoryGroupName;
            entity.GroupRate = dto.GroupRate;
            entity.OwnerCode = dto.OwnerRef.Id.Value;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}