using DoubleGis.Erm.BL.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Clients
{
    public class CheckClientActivitiesHandlerTest : UseModelEntityHandlerTestBase<Client, CheckClientActivitiesRequest, EmptyResponse>
    {
        private readonly long _reserveUserId;

        public CheckClientActivitiesHandlerTest(IPublicService publicService,
                                                IAppropriateEntityProvider<Client> appropriateEntityProvider,
                                                ISecurityServiceUserIdentifier sesSecurityServiceUserIdentifier)
            : base(publicService, appropriateEntityProvider)
        {
            _reserveUserId = sesSecurityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }

        protected override FindSpecification<Client> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<Client>(x => x.OwnerCode != _reserveUserId); }
        }

        protected override bool TryCreateRequest(Client modelEntity, out CheckClientActivitiesRequest request)
        {
            request = new CheckClientActivitiesRequest
                {
                    ClientId = modelEntity.Id
                };

            return true;
        }
    }
}