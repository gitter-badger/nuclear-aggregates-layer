using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class DeactivateOperationServiceTest : UseModelEntityTestBase<LegalPerson>
    {
        private readonly IDeactivateGenericEntityService<LegalPerson> _deactivateGenericEntityService;

        public DeactivateOperationServiceTest(
            IDeactivateGenericEntityService<LegalPerson> deactivateGenericEntityService,
            IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _deactivateGenericEntityService = deactivateGenericEntityService;
        }

        protected override FindSpecification<LegalPerson> ModelEntitySpec
        {
            get
            {
                return Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                       && !LegalPersonSpecs.LegalPersons.Find.InSyncWith1C()
                       && LegalPersonSpecs.LegalPersons.Find.WithoutActiveOrders();
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(LegalPerson modelEntity)
        {
            const long RequiredOwnerId = 1;

            return Result
                .When(_deactivateGenericEntityService.Deactivate(modelEntity.Id, RequiredOwnerId))
                .Then(result => result.Should().BeNull());
        }
    }
}