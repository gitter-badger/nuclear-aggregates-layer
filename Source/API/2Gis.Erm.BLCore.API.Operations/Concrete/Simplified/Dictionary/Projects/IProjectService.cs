using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects.DTO;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects
{
    public interface IProjectService : ISimplifiedModelConsumer
    {
        void CreateOrUpdate(Project project);
        void CreateOrUpdate(IEnumerable<ImportProjectDTO> projects);

        // TODO {d.ivanov, 03.12.2013}: ReadModel
        IEnumerable<Project> GetProjectsByOrganizationUnit(long organizationUnitId);
        Project GetProjectByCode(long projectCode);
    }
}
