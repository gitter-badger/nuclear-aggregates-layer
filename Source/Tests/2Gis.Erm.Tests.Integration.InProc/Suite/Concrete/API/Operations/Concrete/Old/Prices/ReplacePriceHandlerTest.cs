using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Prices
{
    public class ReplacePriceHandlerTest : UseModelEntityHandlerTestBase<Price, ReplacePriceRequest, EmptyResponse>
    {
        private readonly IAppropriateEntityProvider<Price> _appropriateEntityProvider;

        public ReplacePriceHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Price> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            _appropriateEntityProvider = appropriateEntityProvider;
        }

        protected override FindSpecification<Price> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<Price>(p => !p.IsPublished); }
        }

        protected override bool TryCreateRequest(Price modelEntity, out ReplacePriceRequest request)
        {
            request = null;
            var targetPrice = _appropriateEntityProvider.Get(ModelEntitySpec && new FindSpecification<Price>(p => p.Id != modelEntity.Id));

            if (targetPrice == null)
            {
                return false;
            }

            request = new ReplacePriceRequest
                {
                    SourcePriceId = modelEntity.Id,
                    TargetPriceId = targetPrice.Id
                };

            return true;
        }
    }
}