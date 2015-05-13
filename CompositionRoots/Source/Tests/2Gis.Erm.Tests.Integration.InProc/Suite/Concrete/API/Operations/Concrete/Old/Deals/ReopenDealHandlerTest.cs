using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Deals
{
    public class ReopenDealHandlerTest : UseModelEntityHandlerTestBase<Deal, ReopenDealRequest, ReopenDealResponse>
    {
        public ReopenDealHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Deal> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Deal> ModelEntitySpec
        {
            get { return Specs.Find.InactiveAndNotDeletedEntities<Deal>(); }
        }

        protected override bool TryCreateRequest(Deal modelEntity, out ReopenDealRequest request)
        {
            request = new ReopenDealRequest
                {
                    DealId = modelEntity.Id
                };

            return true;
        }
    }
}