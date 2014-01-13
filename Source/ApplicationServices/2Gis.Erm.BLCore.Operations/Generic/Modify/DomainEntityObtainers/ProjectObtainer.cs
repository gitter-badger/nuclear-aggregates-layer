using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

            var entity =
                dto.Id == 0
                    ? new Project()
                    : _finder.Find(Specs.Find.ById<Project>(dto.Id)).Single();

            entity.OrganizationUnitId = dto.OrganizationUnitRef.Id;

            return entity;
        }
    }
}
