using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.GeoClassifier;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly IFinder _finder;
        private readonly IRepository<Project> _projectGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ProjectService(
            IRepository<Project> projectRepository,
            IFinder finder,
            IOperationScopeFactory scopeFactory)
        {
            _projectGenericRepository = projectRepository;
            _finder = finder;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<Project> GetProjectsByOrganizationUnit(long organizationUnitId)
        {
            return _finder.FindMany(ProjectSpecs.Find.ByOrganizationUnit(organizationUnitId) && Specs.Find.Active<Project>());
        }

        public Project GetProjectByCode(long projectCode)
        {
            return _finder.FindOne(Specs.Find.ById<Project>(projectCode));
        }

        public bool DoesActiveProjectExist(long projectCode)
        {
            return _finder.Find(Specs.Find.ById<Project>(projectCode) && Specs.Find.Active<Project>()).Any();
        }

        public void Update(Project project)
        {
            _projectGenericRepository.Update(project);
            _projectGenericRepository.Save();
        }

        public void CreateOrUpdate(IEnumerable<BranchServiceBusDto> projects)
        {
            foreach (var projectDto in projects)
            {
                var project = _finder.Find(Specs.Find.ById<Project>(projectDto.Code)).SingleOrDefault() ??
                              new Project { Id = projectDto.Code };

                project.DisplayName = projectDto.DisplayName;
                project.NameLat = projectDto.NameLat;
                project.IsActive = !projectDto.IsDeleted;
                project.DefaultLang = projectDto.DefaultLang;

                if (project.IsNew())
                {
                    using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Project>())
                    {
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