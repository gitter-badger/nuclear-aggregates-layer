using System;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public static class RequestHandlerUtils
    {
        private static readonly Type TypeOfIRequestHandlerType = typeof(IRequestHandler);
        public static bool IsRequestHandler(this Type checkingType)
        {
            return
                checkingType != TypeOfIRequestHandlerType 
                && TypeOfIRequestHandlerType.IsAssignableFrom(checkingType)
                && !checkingType.IsAbstract
                && (checkingType.BaseType == null || checkingType.BaseType.IsGenericType);
        }

        public static Type GetRequestType(Type handlerType)
        {
            var originType = typeof(RequestHandler<,>);

            for (var baseType = handlerType.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                if (!baseType.IsGenericType)
                {
                    continue;
                }

                if (baseType.GetGenericTypeDefinition() == originType)
                {
                    var result = baseType.GetGenericArguments()[0];

                    // keep generic definitions for generic handlers
                    if (handlerType.IsGenericTypeDefinition)
                    {
                        result = result.GetGenericTypeDefinition();
                    }

                    return result;
                }
            }

            return null;
        }
    }
}
