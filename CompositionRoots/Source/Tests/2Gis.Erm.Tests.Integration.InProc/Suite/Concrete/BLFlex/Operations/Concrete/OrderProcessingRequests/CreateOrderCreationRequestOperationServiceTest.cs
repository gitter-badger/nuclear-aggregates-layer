using System;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class CreateOrderCreationRequestOperationServiceTest : UseModelEntityTestBase<Order>
    {
        private readonly ICreateOrderCreationRequestOperationService _createOrderCreationRequestOperationService;

        public CreateOrderCreationRequestOperationServiceTest(IAppropriateEntityProvider<Order> appropriateEntityProvider,
                                                              ICreateOrderCreationRequestOperationService createOrderCreationRequestOperationService)
            : base(appropriateEntityProvider)
        {
            _createOrderCreationRequestOperationService = createOrderCreationRequestOperationService;
        }
          
        protected override FindSpecification<Order> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<Order>(x => x.LegalPersonProfileId != null); }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Order modelEntity)
        {
            return Result.When(_createOrderCreationRequestOperationService.CreateOrderRequest(modelEntity.SourceOrganizationUnitId,
                                                                                              DateTime.Now.GetNextMonthFirstDate(),
                                                                                              4,
                                                                                              modelEntity.FirmId,
                                                                                              // ReSharper disable once PossibleInvalidOperationException
                                                                                              (long)modelEntity.LegalPersonProfileId,
                                                                                              "Test"))
                         .Then(r => r.Should().NotBe(0));
        }
    }
}