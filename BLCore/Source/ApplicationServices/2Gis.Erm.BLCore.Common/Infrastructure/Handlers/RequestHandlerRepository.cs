using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public class RequestHandlerRepository : IRequestHandlerRepository
    {
        private readonly string _handlersScope;
        private readonly Dictionary<Type, Type> _requestHadlersTypeCache = new Dictionary<Type, Type>();
        
        public RequestHandlerRepository(Type[] handlers, string handlersScope)
        {
            _handlersScope = handlersScope;
            if (handlers.Any(x => !x.IsRequestHandler()))
            {
                throw new ArgumentException("Type of handlers should be IRequestHandler<,>", "handlers");
            }

            foreach (var requestHandlerType in handlers)
            {
                var requestType = RequestHandlerUtils.GetRequestType(requestHandlerType);
                if (requestType == null)
                {
                    continue;
                }

                _requestHadlersTypeCache.Add(requestType, requestHandlerType);
            }
        }

        public Tuple<Type, string> ResolveHandlerType(Type requestType)
        {
            // try find handler for concrete request
            Type requestHandlerType;
            if (!_requestHadlersTypeCache.TryGetValue(requestType, out requestHandlerType))
            {
                if (!requestType.IsGenericType)
                {
                    throw new Exception(string.Format("Cannot find handler for request {0}", requestType.FullName));
                }

                // try find handler for generic request
                var genericRequestType = requestType.GetGenericTypeDefinition();
                if (!_requestHadlersTypeCache.TryGetValue(genericRequestType, out requestHandlerType))
                {
                    throw new Exception(string.Format("Cannot find handler for request {0}", requestType.FullName));
                }

                requestHandlerType = requestHandlerType.MakeGenericType(requestType.GetGenericArguments());
            }

            return new Tuple<Type, string>(requestHandlerType, _handlersScope);
        }
    }
}
