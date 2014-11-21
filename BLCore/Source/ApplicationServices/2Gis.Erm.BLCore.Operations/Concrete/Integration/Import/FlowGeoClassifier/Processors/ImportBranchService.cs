using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.GeoClassifier;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Project;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowGeoClassifier.Processors
{
    public class ImportBranchService : IImportBranchService
    {
        private readonly IProjectService _projectService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportBranchService(IProjectService projectService, IOperationScopeFactory scopeFactory)
        {
            _projectService = projectService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var branchServiceBusDtos = dtos.Cast<BranchServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportBranchIdentity>())
            {
                _projectService.CreateOrUpdate(branchServiceBusDtos);
                scope.Complete();
            }
        }
    }
}