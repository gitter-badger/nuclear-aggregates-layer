using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Limits
{
    public class RecalculateLimitHandlerTest : UseModelEntityHandlerTestBase<Limit, RecalculateLimitRequest, EmptyResponse>
    {
        public RecalculateLimitHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Limit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Limit modelEntity, out RecalculateLimitRequest request)
        {
            request = new RecalculateLimitRequest
                {
                    LimitId = modelEntity.Id
                };

            return true;
        }
    }
}