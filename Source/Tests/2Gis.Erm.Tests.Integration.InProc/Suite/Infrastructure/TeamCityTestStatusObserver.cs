using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class TeamCityTestStatusObserver : ITestStatusObserver
    {
        // Makes use of TeamCity's support for Service Messages
        // http://confluence.jetbrains.com/display/TCD8/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ReportingTests

        public void Started(Type testType)
        {
            SendToTeamCity(string.Format(@"testStarted name='{0}'", ResolveTestId(testType)));
        }

        public void Unresolved(Type testType, Exception exception)
        {
            string description = string.Format("{0}", exception.Message);
            SendToTeamCity(string.Format(@"testFailed name='{0}' message='Unresolved. Can't create test instance' details='{1}'", ResolveTestId(testType), description));
            FinishTest(testType);
        }

        public void Unhandled(Type testType, Exception exception)
        {
            string description = string.Format("{0}", exception.Message + exception.StackTrace);
            SendToTeamCity(string.Format(@"testFailed name='{0}' message='Unhandled. Exception caught {1}' details='{2}'", ResolveTestId(testType), exception.GetType().Name, description));
            FinishTest(testType);
        }

        public void Asserted(Type testType, ITestResult testResult)
        {
            string description = string.Format("{0}", testResult.Asserted != null ? testResult.Asserted.Message : testResult.Report);
            SendToTeamCity(string.Format(@"testFailed type='comparisonFailure' name='{0}' message='Asserted. ' details='{1}' expected='Succeeded' actual='Failed'", ResolveTestId(testType), description));
            FinishTest(testType);
        }

        public void Succeeded(Type testType, ITestResult testResult)
        {
            FinishTest(testType);
        }

        public void Ignored(Type testType, ITestResult testResult)
        {
            string description = string.Format("{0}", testResult.Asserted != null ? testResult.Asserted.Message : testResult.Report);
            SendToTeamCity(string.Format(@"testIgnored name='{0}' details='{1}'", ResolveTestId(testType), description));
        }

        private static void FinishTest(Type testType)
        {
            SendToTeamCity(string.Format(@"testFinished name='{0}'", ResolveTestId(testType)));
        }

        private static string ResolveTestId(Type testType)
        {
            return testType.Name;
        }

        // TeamCity не распознает сообщения с переносами строк
        private static void SendToTeamCity(string message)
        {
            Console.WriteLine(string.Format("##teamcity[{0}]", message).Replace(Environment.NewLine, " "));
        }
    }
}
