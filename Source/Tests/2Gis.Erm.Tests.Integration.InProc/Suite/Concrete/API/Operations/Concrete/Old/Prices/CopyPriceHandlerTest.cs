using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Prices
{
    public class CopyPriceHandlerTest : UseModelEntityHandlerTestBase<Price, CopyPriceRequest, CopyPriceResponse>
    {
        public CopyPriceHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Price> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Price modelEntity, out CopyPriceRequest request)
        {
            var date = DateTime.UtcNow.AddMonths(1).GetFirstDateOfMonth();
            request = new CopyPriceRequest
                {
                    SourcePriceId = modelEntity.Id,
                    BeginDate = date,
                    PublishDate = date,
                    OrganizationUnitId = modelEntity.OrganizationUnitId
                };

            return true;
        }
    }
}