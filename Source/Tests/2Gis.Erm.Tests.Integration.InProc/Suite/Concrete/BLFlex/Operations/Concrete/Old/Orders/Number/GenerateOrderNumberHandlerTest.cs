using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.Old.Orders.Number
{
    public class GenerateOrderNumberHandlerTest :
        UseModelEntityHandlerTestBase<Order, GenerateOrderNumberRequest, GenerateOrderNumberResponse>
    {
        public GenerateOrderNumberHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Order> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Order modelEntity, out GenerateOrderNumberRequest request)
        {
            request = new GenerateOrderNumberRequest
                {
                    IsRegionalNumber = modelEntity.SourceOrganizationUnitId != modelEntity.DestOrganizationUnitId,
                    Order = modelEntity
                };

            return true;
        }

        protected override IResponseAsserter<GenerateOrderNumberResponse> ResponseAsserter
        {
            get
            {
                return new DelegateResponseAsserter<GenerateOrderNumberResponse>(Assert);
            }
        }

        private static OrdinaryTestResult Assert(GenerateOrderNumberResponse x)
        {
            return Result.When(x).Then(r => r.Should().NotBeNull());
        }
    }
}