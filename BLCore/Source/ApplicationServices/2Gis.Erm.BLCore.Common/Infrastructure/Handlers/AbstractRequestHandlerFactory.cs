using System;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public abstract class AbstractRequestHandlerFactory : IRequestHandlerFactory
    {
        private readonly IRequestHandlerRepository _handlerRepository;

        protected AbstractRequestHandlerFactory(IRequestHandlerRepository handlerRepository)
        {
            _handlerRepository = handlerRepository;
        }

        public IRequestHandler GetHandler(Type requestType)
        {
            var handlerDescription = _handlerRepository.ResolveHandlerType(requestType);
            return GetHandlerInternal(handlerDescription.Item1, handlerDescription.Item2);
        }

        protected abstract IRequestHandler GetHandlerInternal(Type handlerType, string handlerScope);
    }
}