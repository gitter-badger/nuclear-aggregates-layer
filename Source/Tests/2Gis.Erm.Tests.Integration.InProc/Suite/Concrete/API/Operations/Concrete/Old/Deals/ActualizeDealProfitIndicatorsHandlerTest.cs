using DoubleGis.Erm.Core.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Deals
{
    public class ActualizeDealProfitIndicatorsHandlerTest : UseModelEntityHandlerTestBase<Deal, ActualizeDealProfitIndicatorsRequest, EmptyResponse>
    {
        public ActualizeDealProfitIndicatorsHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Deal> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Deal modelEntity, out ActualizeDealProfitIndicatorsRequest request)
        {
            request = new ActualizeDealProfitIndicatorsRequest
                {
                    DealIds = new[] { modelEntity.Id }
                };

            return true;
        }
    }
}