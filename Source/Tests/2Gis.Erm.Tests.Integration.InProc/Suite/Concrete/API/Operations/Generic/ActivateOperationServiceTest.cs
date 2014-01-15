using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ActivateOperationServiceTest : UseModelEntityTestBase<LegalPerson>
    {
        private readonly IActivateGenericEntityService<LegalPerson> _activateGenericEntityService;
        
        public ActivateOperationServiceTest(
            IActivateGenericEntityService<LegalPerson> activateGenericEntityService,
            IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _activateGenericEntityService = activateGenericEntityService;
        }

        protected override FindSpecification<LegalPerson> ModelEntitySpec
        {
            get { return Specs.Find.InactiveEntities<LegalPerson>(); }
        }

        protected override OrdinaryTestResult ExecuteWithModel(LegalPerson modelEntity)
        {
            return Result
                    .When(_activateGenericEntityService.Activate(modelEntity.Id))
                    .Then(activated => activated.Should().BeGreaterThan(0));
        }
    }
}