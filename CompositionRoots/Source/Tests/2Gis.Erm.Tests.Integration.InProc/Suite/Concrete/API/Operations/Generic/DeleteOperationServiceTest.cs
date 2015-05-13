using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class DeleteOperationServiceTest : UseModelEntityTestBase<LegalPerson>
    {
        private readonly IDeleteGenericEntityService<LegalPerson> _deleteGenericEntityService;

        public DeleteOperationServiceTest(
            IDeleteGenericEntityService<LegalPerson> deleteGenericEntityService,
            IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _deleteGenericEntityService = deleteGenericEntityService;
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
            return Result
                .When(_deleteGenericEntityService.Delete(modelEntity.Id))
                .Then(confirm => confirm.Should().BeNull());
        }
    }
}