using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Clients
{
    public class MergeClientsHandlerTest : UseModelEntityHandlerTestBase<Client, MergeClientsRequest, EmptyResponse>
    {
        private readonly IAppropriateEntityProvider<Client> _appropriateEntityProvider;
        private readonly long _reserveUserId;

        public MergeClientsHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Client> appropriateEntityProvider, ISecurityServiceUserIdentifier sesSecurityServiceUserIdentifier)
            : base(publicService, appropriateEntityProvider)
        {
            _appropriateEntityProvider = appropriateEntityProvider;
            _reserveUserId = sesSecurityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }

        protected override FindSpecification<Client> ModelEntitySpec
        {
            get { return Specs.Find.ActiveAndNotDeleted<Client>() && new FindSpecification<Client>(c => c.OwnerCode != _reserveUserId); }
        }

        protected override bool TryCreateRequest(Client modelEntity, out MergeClientsRequest request)
        {
            request = null;

            var appendedClient = _appropriateEntityProvider.Get(ModelEntitySpec && new FindSpecification<Client>(x => x.Id != modelEntity.Id));
            if (appendedClient == null)
            {
                return false;
            }

            request = new MergeClientsRequest
                {
                    AppendedClientId = appendedClient.Id,
                    Client = modelEntity
                };

            return true;
        }
    }
}