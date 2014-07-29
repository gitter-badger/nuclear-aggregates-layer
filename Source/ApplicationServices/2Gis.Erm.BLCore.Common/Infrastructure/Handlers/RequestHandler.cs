using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandlerInternal
        where TRequest : Request
        where TResponse : Response
    {
        protected HandlerContext Context { get; private set; }
        HandlerContext IRequestHandlerInternal.Context
        {
            set
            {
                Context = value;
            }
        }

        protected abstract TResponse Handle(TRequest request);

        public Response Handle(Request request)
        {
            return Handle((TRequest)request);
        }
    }
}