using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Deals
{
    public class CheckDealHandlerTest : UseModelEntityHandlerTestBase<Deal, CheckDealRequest, EmptyResponse>
    {
        public CheckDealHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Deal> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Deal modelEntity, out CheckDealRequest request)
        {
            request = new CheckDealRequest
                {
                    DealId = modelEntity.Id,
                    CurrencyId = modelEntity.CurrencyId
                };

            return true;
        }
    }
}