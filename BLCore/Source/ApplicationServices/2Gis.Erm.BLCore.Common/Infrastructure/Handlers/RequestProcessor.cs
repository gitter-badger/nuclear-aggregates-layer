using System;

using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public sealed class RequestProcessor : IRequestProcessor, ISubRequestProcessor
    {
        private readonly IRequestHandlerFactory _requestHandlerFactory;
        private readonly ICommonLog _logger;

        public RequestProcessor(IRequestHandlerFactory requestHandlerFactory, ICommonLog logger)
        {
            _requestHandlerFactory = requestHandlerFactory;
            _logger = logger;
        }

        public Response HandleRequest(Request request)
        {
            return HandleRequest(request, null, null);
        }

        public Response HandleSubRequest(Request subRequest, HandlerContext parentContext)
        {
            return HandleRequest(subRequest, parentContext, null);
        }

        public Response HandleSubRequest(Request subRequest, HandlerContext parentContext, bool isParentShareTransaction)
        {
            return HandleRequest(subRequest, parentContext, isParentShareTransaction);
        }

        private ResolvedHandlerDescriptor GetHandlerForRequest(Type requestType, HandlerContext parentContext)
        {
            var handlerDescriptor = new ResolvedHandlerDescriptor();
            handlerDescriptor.Handler = _requestHandlerFactory.GetHandler(requestType);

            if (handlerDescriptor.Handler == null)
            {
                throw new BusinessLogicException(string.Format("Request handler not found for request: '{0}'", requestType.Name));
            }

            var handlerInternal = handlerDescriptor.Handler as IRequestHandlerInternal;
            if (handlerInternal == null)
            {
                throw new NotSupportedException(string.Format("Handler for specified request: '{0}' is not support context", requestType.Name));
            }

            handlerDescriptor.HandlerContextAccessor = handlerInternal;

            return handlerDescriptor;
        }

        private Response HandleRequest(Request request, HandlerContext parentContext, bool? isParentShareTransaction)
        {
            var requestType = request.GetType();
            var resolvedHandlerDescriptor = GetHandlerForRequest(requestType, parentContext);
            return HandleRequest(resolvedHandlerDescriptor, request);
        }

        private Response HandleRequest(ResolvedHandlerDescriptor handlerDescriptor, Request request)
        {
            var requestName = request.GetType().Name;
            var handlerType = handlerDescriptor.Handler.GetType();
            var handlerContext = new HandlerContext(handlerType);

            handlerDescriptor.HandlerContextAccessor.Context = handlerContext;

            try
            {
                var response = handlerDescriptor.Handler.Handle(request);
                _logger.DebugFormat("Обработка запроса: '{0}' прошла успешно", requestName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "При обработке запроса: '{0}' произошла ошибка", requestName);
                throw;
            }
        }

        private class ResolvedHandlerDescriptor
        {
            public IRequestHandler Handler { get; set; }
            public IRequestHandlerInternal HandlerContextAccessor { get; set; }
        }
    }
}