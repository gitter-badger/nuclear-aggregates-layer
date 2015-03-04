using System;
using System.IO;
using System.Text;
using System.Windows;

using DoubleGis.Platform.UI.WPF.Shell;
using DoubleGis.Platform.UI.WPF.Shell.DI;

using Nuclear.Tracing.API.SystemInfo;
using Nuclear.Tracing.Log4Net.Config;

namespace DoubleGis.Erm.UI.Desktop.WPF
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var logger = Log4NetTracerBuilder.Use
                                             .XmlConfig(Path.Combine(Bootstrapper.GetApplicationWorkingDirectory, Log4NetTracerBuilder.DefaultLogConfigFileName))
                                             .File("Erm.WPF.Client")
                                             .Build; 

            logger.Info("Application starting ...");

            logger.Info(
                new StringBuilder().AppendLine("Environment info:" + EnvironmentInfo.Description)
                                   .AppendLine("User info:" + SecurityInfo.UserSecuritySettingsDescription)
                                   .AppendLine("Network info:" + NetworkInfo.DomainMembership)
                                   .ToString());

            var app = new App(logger);

            logger.Info("Application started successfully");
            logger.Info("Application run ...");

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application error. Appeal to ERM support team", "ERM WPF Client");
                return;
            }

            logger.Info("Application finished");
        }
    }
}
