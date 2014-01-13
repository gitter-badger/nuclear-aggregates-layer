using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public sealed class PublicService : IPublicService
    {
        private readonly IRequestProcessor _requestProcessor;
        
        public PublicService(IRequestProcessor requestProcessor)
        {
            _requestProcessor = requestProcessor;
        }

        Response IPublicService.Handle(Request request)
        {
            return _requestProcessor.HandleRequest(request);
        }
    }
}