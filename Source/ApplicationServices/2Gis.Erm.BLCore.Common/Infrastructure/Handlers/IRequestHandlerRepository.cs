using System;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public interface IRequestHandlerRepository
    {
        Tuple<Type, string> ResolveHandlerType(Type requestType);
    }
}