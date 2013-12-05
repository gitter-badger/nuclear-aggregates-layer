using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public static class Result
    {
        public static TResult When<TResult>(TResult result)
        {
            return result;
        }

        public static OrdinaryTestResult When(Action action)
        {
            try
            {
                action();
                return OrdinaryTestResult.As.Succeeded;
            }
            catch (Exception ex)
            {
                return OrdinaryTestResult.As.Unhandled(ex);
            }
        }

        public static OrdinaryTestResult Then(this OrdinaryTestResult halfFinishedResult, Func<bool> predicate)
        {
            return predicate() ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }

        public static OrdinaryTestResult Then<TResult>(this TResult result, Func<TResult, bool> predicate)
        {
            return predicate(result) ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }

        public static OrdinaryTestResult Then(this OrdinaryTestResult halfFinishedResult, Action evaluator)
        {
            try
            {
                evaluator();
                return OrdinaryTestResult.As.Succeeded;
            }
            catch (Exception ex)
            {
                return OrdinaryTestResult.As.Asserted(ex);
            }
        }

        public static OrdinaryTestResult Then<TResult>(this TResult result, Action<TResult> evaluator)
        {
            try
            {
                evaluator(result);
                return OrdinaryTestResult.As.Succeeded;
            }
            catch (Exception ex)
            {
                return OrdinaryTestResult.As.Asserted(ex);
            }
        }
    }
}