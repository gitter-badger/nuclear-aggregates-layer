using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementTemplates;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.AdvertisementTemplates
{
    public class PublishAdvertisementTemplateHandlerTest :
        UseModelEntityHandlerTestBase<AdvertisementTemplate, PublishAdvertisementTemplateRequest, EmptyResponse>
    {
        public PublishAdvertisementTemplateHandlerTest(IPublicService publicService, IAppropriateEntityProvider<AdvertisementTemplate> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<AdvertisementTemplate> ModelEntitySpec
        {
            get
            {
                return Specs.Find.NotDeleted<AdvertisementTemplate>() &&
                       new FindSpecification<AdvertisementTemplate>(
                           at => !at.IsDeleted && at.AdsTemplatesAdsElementTemplates.Any(ataet => !ataet.IsDeleted) &&
                                 !at.Advertisements.Any(
                                     a =>
                                     !a.IsDeleted && a.FirmId == null &&
                                     a.AdvertisementElements.Any(
                                         ae =>
                                         !ae.IsDeleted && ((ae.BeginDate == null || ae.EndDate == null) && ae.FileId == null && string.IsNullOrEmpty(ae.Text)))));
            }
        }

        protected override bool TryCreateRequest(AdvertisementTemplate modelEntity, out PublishAdvertisementTemplateRequest request)
        {
            request = new PublishAdvertisementTemplateRequest
                {
                    AdvertisementTemplateId = modelEntity.Id
                };

            return true;
        }
    }
}