using System;
using System.IO;
using System.Text;
using System.Windows;

using DoubleGis.Platform.UI.WPF.Shell;
using DoubleGis.Platform.UI.WPF.Shell.DI;

using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;

namespace DoubleGis.Erm.UI.Desktop.WPF
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var tracer = Log4NetTracerBuilder.Use
                                             .XmlConfig(Path.Combine(Bootstrapper.GetApplicationWorkingDirectory, Log4NetTracerBuilder.DefaultTracerConfigFileName))
                                             .File("Erm.WPF.Client")
                                             .Build; 

            tracer.Info("Application starting ...");

            tracer.Info(
                new StringBuilder().AppendLine("Environment info:" + EnvironmentInfo.Description)
                                   .AppendLine("User info:" + SecurityInfo.UserSecuritySettingsDescription)
                                   .AppendLine("Network info:" + NetworkInfo.DomainMembership)
                                   .ToString());

            var app = new App(tracer);

            tracer.Info("Application started successfully");
            tracer.Info("Application run ...");

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application error. Appeal to ERM support team", "ERM WPF Client");
                return;
            }

            tracer.Info("Application finished");
        }
    }
}
