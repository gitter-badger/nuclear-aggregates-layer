using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Prices
{
    public class UnpublishPriceHandlerTest : UseModelEntityHandlerTestBase<Price, UnpublishPriceRequest, EmptyResponse>
    {
        public UnpublishPriceHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Price> appropriateEntityProvider) : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Price> ModelEntitySpec
        {
            get
            {
                var startOfNextMounth = DateTime.UtcNow.GetNextMonthFirstDate();
                return base.ModelEntitySpec &&
                       new FindSpecification<Price>(
                           p =>
                           p.IsPublished &&
                           p.OrganizationUnit.Prices.Any(
                               pp =>
                               pp.Id != p.Id && pp.IsActive && !pp.IsDeleted && pp.IsPublished && pp.BeginDate > p.BeginDate && pp.BeginDate < startOfNextMounth));
            }
        }

        protected override bool TryCreateRequest(Price modelEntity, out UnpublishPriceRequest request)
        {
            request = new UnpublishPriceRequest
                {
                    PriceId = modelEntity.Id
                };

            return true;
        }
    }
}