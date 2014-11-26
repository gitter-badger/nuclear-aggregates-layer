using System;
using System.IO;
using System.Text;
using System.Windows;

using DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config;
using DoubleGis.Erm.Platform.Common.Logging.SystemInfo;
using DoubleGis.Platform.UI.WPF.Shell;
using DoubleGis.Platform.UI.WPF.Shell.DI;

namespace DoubleGis.Erm.UI.Desktop.WPF
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var logger = Log4NetLoggerBuilder.Use
                                             .XmlConfig(Path.Combine(Bootstrapper.GetApplicationWorkingDirectory, Log4NetLoggerBuilder.DefaultLogConfigFileName))
                                             .File("Erm.WPF.Client")
                                             .Build; 

            logger.InfoEx("Application starting ...");

            logger.InfoEx(
                new StringBuilder().AppendLine("Environment info:" + EnvironmentInfo.Description)
                                   .AppendLine("User info:" + SecurityInfo.UserSecuritySettingsDescription)
                                   .AppendLine("Network info:" + NetworkInfo.DomainMembership)
                                   .ToString());

            var app = new App(logger);

            logger.InfoEx("Application started successfully");
            logger.InfoEx("Application run ...");

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application error. Appeal to ERM support team", "ERM WPF Client");
                return;
            }

            logger.InfoEx("Application finished");
        }
    }
}
