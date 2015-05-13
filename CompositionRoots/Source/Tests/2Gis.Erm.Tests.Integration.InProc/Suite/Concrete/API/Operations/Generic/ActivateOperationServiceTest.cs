using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ActivateOperationServiceTest : UseModelEntityTestBase<BranchOfficeOrganizationUnit>
    {
        private readonly IActivateGenericEntityService<BranchOfficeOrganizationUnit> _activateGenericEntityService;
        
        public ActivateOperationServiceTest(
            IActivateGenericEntityService<BranchOfficeOrganizationUnit> activateGenericEntityService,
            IAppropriateEntityProvider<BranchOfficeOrganizationUnit> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _activateGenericEntityService = activateGenericEntityService;
        }

        protected override FindSpecification<BranchOfficeOrganizationUnit> ModelEntitySpec
        {
            get { return Specs.Find.InactiveAndNotDeletedEntities<BranchOfficeOrganizationUnit>(); }
        }

        protected override OrdinaryTestResult ExecuteWithModel(BranchOfficeOrganizationUnit modelEntity)
        {
            return Result
                    .When(_activateGenericEntityService.Activate(modelEntity.Id))
                    .Then(activated => activated.Should().BeGreaterThan(0));
        }
    }
}