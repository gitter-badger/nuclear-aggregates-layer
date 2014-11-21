using System;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public interface IClientProxy<out TChannel>
    {
        void Execute(Action<TChannel> action);
        TResult Execute<TResult>(Func<TChannel, TResult> func);
        bool TryExecuteWithFaultContract<TResult>(Func<TChannel, TResult> func, out TResult result, out object faultContract);
    }
}