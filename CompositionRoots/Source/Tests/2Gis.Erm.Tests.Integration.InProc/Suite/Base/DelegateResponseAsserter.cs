using System;

using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Base
{
    public class DelegateResponseAsserter<T> : IResponseAsserter<T>
    {
        private readonly Func<T, OrdinaryTestResult> _assertFunc;

        public DelegateResponseAsserter()
        {
            _assertFunc = r => OrdinaryTestResult.As.Succeeded;
        }

        public DelegateResponseAsserter(Func<T, OrdinaryTestResult> assertFunc)
        {
            _assertFunc = assertFunc;
        }

        public OrdinaryTestResult Assert(T result)
        {
            return _assertFunc(result);
        }
    }
}