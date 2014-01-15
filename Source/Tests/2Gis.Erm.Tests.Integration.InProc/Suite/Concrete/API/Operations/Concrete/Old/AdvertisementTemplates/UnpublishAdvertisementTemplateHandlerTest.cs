using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementTemplates;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.AdvertisementTemplates
{
    public class UnpublishAdvertisementTemplateHandlerTest :
        UseModelEntityHandlerTestBase<AdvertisementTemplate, UnpublishAdvertisementTemplateRequest, EmptyResponse>
    {
        public UnpublishAdvertisementTemplateHandlerTest(IPublicService publicService,
                                                         IAppropriateEntityProvider<AdvertisementTemplate> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(AdvertisementTemplate modelEntity, out UnpublishAdvertisementTemplateRequest request)
        {
            request = new UnpublishAdvertisementTemplateRequest
                {
                    AdvertisementTemplateId = modelEntity.Id
                };

            return true;
        }
    }
}