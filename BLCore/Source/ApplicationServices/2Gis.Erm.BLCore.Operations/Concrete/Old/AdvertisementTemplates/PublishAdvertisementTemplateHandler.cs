using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementTemplates;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.AdvertisementTemplates
{
    public sealed class PublishAdvertisementTemplateHandler : RequestHandler<PublishAdvertisementTemplateRequest, EmptyResponse>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public PublishAdvertisementTemplateHandler(
            IAdvertisementRepository advertisementRepository, 
            ISecurityServiceFunctionalAccess functionalAccessService, 
            IUserContext userContext)
        {
            _advertisementRepository = advertisementRepository;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        protected override EmptyResponse Handle(PublishAdvertisementTemplateRequest request)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.PublishAdvertisementTemplate, _userContext.Identity.Code))
            {
                throw new SecurityException(BLResources.YouHaveNotPublishAdvertisementTemplatesFunctionalPrivelege);
            }

            _advertisementRepository.Publish(request.AdvertisementTemplateId);
            return Response.Empty;
        }
    }
}
