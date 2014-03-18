using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects.DTO;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Project
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Platform.Model.Entities.Erm.Project> _projectGenericRepository;
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;

        public ProjectService(
            IRepository<Platform.Model.Entities.Erm.Project> projectRepository, 
            IFinder finder, 
            IIdentityProvider identityProvider)
        {
            _projectGenericRepository = projectRepository;
            _finder = finder;
            _identityProvider = identityProvider;
        }

        public IEnumerable<Platform.Model.Entities.Erm.Project> GetProjectsByOrganizationUnit(long organizationUnitId)
        {
            return _finder.Find<Platform.Model.Entities.Erm.Project>(x => x.IsActive && x.OrganizationUnitId == organizationUnitId).ToArray();
        }

        public Platform.Model.Entities.Erm.Project GetProjectByCode(long projectCode)
        {
            return _finder.Find(ProjectSpecs.Find.ByCode(projectCode)).SingleOrDefault();
        }

        public void CreateOrUpdate(Platform.Model.Entities.Erm.Project project)
        {
            if (project.IsNew())
            {
                _identityProvider.SetFor(project);
                _projectGenericRepository.Add(project);
            }
            else
            {
                _projectGenericRepository.Update(project);
            }

            _projectGenericRepository.Save();
        }

        public void CreateOrUpdate(IEnumerable<ImportProjectDTO> projects)
        {
            foreach (var projectDto in projects)
            {
                var dto = projectDto;
                var project = _finder.Find<Platform.Model.Entities.Erm.Project>(x => x.Code == dto.Code).SingleOrDefault() ??
                                new Platform.Model.Entities.Erm.Project { Code = projectDto.Code };

                project.DisplayName = projectDto.DisplayName;
                project.NameLat = projectDto.NameLat;
                project.IsActive = !projectDto.IsDeleted;
                project.DefaultLang = projectDto.DefaultLang;

                if (project.IsNew())
                {
                    _identityProvider.SetFor(project);
                    _projectGenericRepository.Add(project);
                }
                else
                {
                    _projectGenericRepository.Update(project);
                }

                _projectGenericRepository.Save();
            }
        }
    }
}
