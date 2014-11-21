using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Projects
{
    public class ProjectServiceTest : UseModelEntityTestBase<Project>
    {
        private readonly IProjectService _projectService;

        public ProjectServiceTest(IAppropriateEntityProvider<Project> appropriateEntityProvider, IProjectService projectService)
            : base(appropriateEntityProvider)
        {
            _projectService = projectService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Project modelEntity)
        {
            modelEntity.DisplayName = "Test";
            _projectService.Update(modelEntity);

            // Кейс работает только в TaskService
            //modelEntity.ResetToNew();
            //_projectService.CreateOrUpdate(modelEntity);

            _projectService.GetProjectsByOrganizationUnit(modelEntity.Id);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}