using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Clients
{
    public class CheckIsWarmClientHandlerTest : UseModelEntityHandlerTestBase<Client, CheckIsWarmClientRequest, CheckIsWarmClientResponse>
    {
        public CheckIsWarmClientHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Client> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Client modelEntity, out CheckIsWarmClientRequest request)
        {
            request = new CheckIsWarmClientRequest
                {
                    ClientId = modelEntity.Id
                };

            return true;
        }
    }
}