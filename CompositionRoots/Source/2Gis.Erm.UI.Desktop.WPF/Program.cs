using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Logging.SystemInfo;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Platform.UI.WPF.Shell;
using DoubleGis.Platform.UI.WPF.Shell.DI;

using log4net.Config;

namespace DoubleGis.Erm.UI.Desktop.WPF
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var logger = CreateLogger(); 

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

        private static ICommonLog CreateLogger()
        {
            var logConfigFileFullPath = Path.Combine(Bootstrapper.GetApplicationWorkingDirectory, LogUtils.DefaultLogConfigFileName);
            XmlConfigurator.Configure(new FileInfo(logConfigFileFullPath));
            return Log4NetImpl.GetLogger(LoggerConstants.Erm);
        }
    }
}
