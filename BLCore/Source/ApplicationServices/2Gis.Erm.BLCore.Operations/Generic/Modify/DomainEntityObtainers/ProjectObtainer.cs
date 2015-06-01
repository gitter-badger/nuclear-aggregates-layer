using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class ProjectObtainer : ISimplifiedModelEntityObtainer<Project>
    {
        private readonly IFinder _finder;

        public ProjectObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Project ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ProjectDomainEntityDto)domainEntityDto;
            var entity = _finder.Find(Specs.Find.ById<Project>(dto.Id)).One();

            if (entity == null)
            {
                throw new NotSupportedException("Project creation is not supported");
            }

            entity.OrganizationUnitId = dto.OrganizationUnitRef.Id;

            return entity;
        }
    }
}
