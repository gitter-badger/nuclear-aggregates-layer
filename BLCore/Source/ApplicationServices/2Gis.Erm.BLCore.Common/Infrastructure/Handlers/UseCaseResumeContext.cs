using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.UseCases;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public sealed class UseCaseResumeContext<TRequest> : IUseCaseResumeContext<TRequest>
        where TRequest : Request
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly HandlerContext _callerHandlerContext;
        private readonly TRequest _request;

        public UseCaseResumeContext(ISubRequestProcessor subRequestProcessor, HandlerContext callerHandlerContext, TRequest request)
        {
            _subRequestProcessor = subRequestProcessor;
            _callerHandlerContext = callerHandlerContext;
            _request = request;
        }

        public TRequest Request
        {
            get
            {
                return _request;
            }
        }

        public Response UseCaseResume(Request subRequest)
        {
            return _subRequestProcessor.HandleSubRequest(subRequest, _callerHandlerContext);
        }

        public Response UseCaseResume(Request subRequest, bool isParentShareTransaction)
        {
            return _subRequestProcessor.HandleSubRequest(subRequest, _callerHandlerContext, isParentShareTransaction);
        }
    }
}
