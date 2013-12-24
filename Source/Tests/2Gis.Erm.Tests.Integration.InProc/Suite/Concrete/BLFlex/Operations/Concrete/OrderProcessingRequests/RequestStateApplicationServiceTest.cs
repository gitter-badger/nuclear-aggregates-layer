﻿using DoubleGis.Erm.BL.API.Operations.Special.Remote.OrderProcessing;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class RequestStateApplicationServiceTest : UseModelEntityTestBase<OrderProcessingRequest>
    {
        private readonly IRequestStateApplicationService _requestStateApplicationService;

        public RequestStateApplicationServiceTest(IAppropriateEntityProvider<OrderProcessingRequest> appropriateEntityProvider,
                                                  IRequestStateApplicationService requestStateApplicationService)
            : base(appropriateEntityProvider)
        {
            _requestStateApplicationService = requestStateApplicationService;
        }

        protected override FindSpecification<OrderProcessingRequest> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<OrderProcessingRequest>(x => x.State != (int)OrderProcessingRequestState.Undefined); }
        }

        protected override OrdinaryTestResult ExecuteWithModel(OrderProcessingRequest modelEntity)
        {
            return Result.When(_requestStateApplicationService.GetState(new[] { modelEntity.Id }))
                         .Then(r => r.Should().NotBeEmpty());
        }
    }
}