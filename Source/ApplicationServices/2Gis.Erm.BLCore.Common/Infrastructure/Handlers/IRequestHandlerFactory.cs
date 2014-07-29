using System;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public interface IRequestHandlerFactory
    {
        IRequestHandler GetHandler(Type requestType);
    }
}