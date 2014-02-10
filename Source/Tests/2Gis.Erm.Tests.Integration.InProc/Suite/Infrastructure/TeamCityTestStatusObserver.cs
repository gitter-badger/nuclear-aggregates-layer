using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class TeamCityTestStatusObserver : ITestStatusObserver
    {
        // Makes use of TeamCity's support for Service Messages
        // http://confluence.jetbrains.com/display/TCD8/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ReportingTests

        public void Started(Type testType)
        {
            Console.WriteLine(new TeamCityMessage("testStarted")
                .WithProperty("name", ResolveTestId(testType)));
        }

        public void Unresolved(Type testType, Exception exception)
        {
            Console.WriteLine(new TeamCityMessage("testFailed")
                .WithProperty("name", ResolveTestId(testType))
                .WithProperty("message", "Unresolved. Can not create test instance")
                .WithProperty("details", exception.Message));

            FinishTest(testType);
        }

        public void Unhandled(Type testType, Exception exception)
        {
            Console.WriteLine(new TeamCityMessage("testFailed")
                .WithProperty("name", ResolveTestId(testType))
                .WithProperty("message", "Unhandled. Exception caught " + exception.GetType().Name)
                .WithProperty("details", exception.Message + exception.StackTrace));

            FinishTest(testType);
        }

        public void Asserted(Type testType, ITestResult testResult)
        {
            Console.WriteLine(new TeamCityMessage("testFailed")
                .WithProperty("name", ResolveTestId(testType))
                .WithProperty("message", "Asserted. ")
                .WithProperty("details", testResult.Asserted != null ? testResult.Asserted.Message : testResult.Report)
                .WithProperty("expected", "Succeeded")
                .WithProperty("actual", "Failed"));

            FinishTest(testType);
        }

        public void Succeeded(Type testType, ITestResult testResult)
        {
            FinishTest(testType);
        }

        public void Ignored(Type testType, ITestResult testResult)
        {
            Console.WriteLine(new TeamCityMessage("testIgnored")
                .WithProperty("name", ResolveTestId(testType))
                .WithProperty("details", testResult.Asserted != null ? testResult.Asserted.Message : testResult.Report));
        }

        private static void FinishTest(Type testType)
        {   
            Console.WriteLine(new TeamCityMessage("testFinished")
                .WithProperty("name", ResolveTestId(testType)));
        }

        private static string ResolveTestId(Type testType)
        {
            return testType.Name;
        }

        private class TeamCityMessage
        {
            private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

            public TeamCityMessage(string header)
            {
                Header = header;
            }

            public string Header { get; private set;  }

            public TeamCityMessage WithProperty(string name, string value)
            {
                _properties.Add(name, value.Replace('\'', '_').Replace(Environment.NewLine, " "));
                return this;
            }

            public override string ToString()
            {
                var properies = string.Join(" ", _properties.Select(x => string.Format("{0}='{1}'", x.Key, x.Value)));

                return string.Format("##teamcity[{0} {1}]", Header, properies);
            }
        }
    }
}
