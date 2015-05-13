using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class CreateOrderProlongationRequestOperationServiceTest : UseModelEntityTestBase<Order>
    {
        private readonly ICreateOrderProlongationRequestOperationService _createOrderProlongationRequestOperationService;

        public CreateOrderProlongationRequestOperationServiceTest(IAppropriateEntityProvider<Order> appropriateEntityProvider,
                                                                  ICreateOrderProlongationRequestOperationService createOrderProlongationRequestOperationService)
            : base(appropriateEntityProvider)
        {
            _createOrderProlongationRequestOperationService = createOrderProlongationRequestOperationService;
        } 

        protected override FindSpecification<Order> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<Order>(x => x.LegalPersonProfileId != null); }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Order modelEntity)
        {
            return Result.When(_createOrderProlongationRequestOperationService.CreateOrderProlongationRequest(modelEntity.Id, 4, "Test"))
                         .Then(r => r.Should().NotBe(0));
        }
    }
}