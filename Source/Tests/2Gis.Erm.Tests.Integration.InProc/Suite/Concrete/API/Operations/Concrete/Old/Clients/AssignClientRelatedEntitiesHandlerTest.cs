using DoubleGis.Erm.BL.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Clients
{
    public class AssignClientRelatedEntitiesHandlerTest : UseModelEntityHandlerTestBase<Client, AssignClientRelatedEntitiesRequest, EmptyResponse>
    {
        private readonly long _reserveUserId;

        public AssignClientRelatedEntitiesHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Client> appropriateEntityProvider, ISecurityServiceUserIdentifier sesSecurityServiceUserIdentifier)
            : base(publicService, appropriateEntityProvider)
        {
            _reserveUserId = sesSecurityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }

        protected override FindSpecification<Client> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<Client>(x => x.OwnerCode != _reserveUserId); }
        }

        protected override bool TryCreateRequest(Client modelEntity, out AssignClientRelatedEntitiesRequest request)
        {
            request = new AssignClientRelatedEntitiesRequest
                {
                    ClientId = modelEntity.Id,
                    IsPartial = false,
                    OwnerCode = 1
                };

            return true;
        }
    }
}