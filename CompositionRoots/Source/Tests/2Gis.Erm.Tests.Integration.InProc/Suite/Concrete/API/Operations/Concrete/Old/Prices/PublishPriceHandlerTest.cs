using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Prices
{
    public class PublishPriceHandlerTest : UseModelEntityHandlerTestBase<Price, PublishPriceRequest, EmptyResponse>
    {
        public PublishPriceHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Price> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Price> ModelEntitySpec
        {
            get
            {
                var startOfNextMonth = DateTime.UtcNow.GetNextMonthFirstDate();
                return base.ModelEntitySpec && new FindSpecification<Price>(p => !p.IsPublished && p.BeginDate > startOfNextMonth);
            }
        }

        protected override bool TryCreateRequest(Price modelEntity, out PublishPriceRequest request)
        {
            request = new PublishPriceRequest
                {
                    BeginDate = DateTime.UtcNow.GetNextMonthFirstDate(),
                    OrganizarionUnitId = modelEntity.OrganizationUnitId,
                    PriceId = modelEntity.Id,
                    PublishDate = DateTime.UtcNow
                };

            return true;
        }
    }
}