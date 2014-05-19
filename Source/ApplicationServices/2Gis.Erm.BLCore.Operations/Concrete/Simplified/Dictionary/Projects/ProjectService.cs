using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.GeoClassifier;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Project> _projectGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ProjectService(
            IRepository<Project> projectRepository,
            IFinder finder,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _projectGenericRepository = projectRepository;
            _finder = finder;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<Project> GetProjectsByOrganizationUnit(long organizationUnitId)
        {
            return _finder.Find<Project>(x => x.IsActive && x.OrganizationUnitId == organizationUnitId).ToArray();
        }

        public Project GetProjectByCode(long projectCode)
        {
            return _finder.Find(ProjectSpecs.Find.ByCode(projectCode)).SingleOrDefault();
        }

        public void CreateOrUpdate(Project project)
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

        public void CreateOrUpdate(IEnumerable<BranchServiceBusDto> projects)
        {
            foreach (var projectDto in projects)
            {
                var dto = projectDto;
                var project = _finder.Find<Project>(x => x.Code == dto.Code).SingleOrDefault() ??
                              new Project { Code = projectDto.Code };

                project.DisplayName = projectDto.DisplayName;
                project.NameLat = projectDto.NameLat;
                project.IsActive = !projectDto.IsDeleted;
                project.DefaultLang = projectDto.DefaultLang;

                if (project.IsNew())
                {
                    using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Project>())
                    {
                        _identityProvider.SetFor(project);
                        _projectGenericRepository.Add(project);
                        scope.Added<Project>(project.Id)
                             .Complete();
                    }
                }
                else
                {
                    using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Project>())
                    {
                        _projectGenericRepository.Update(project);
                        scope.Updated<Project>(project.Id)
                             .Complete();
                    }
                }

                _projectGenericRepository.Save();
            }
        }
    }
}