using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;
using DoubleGis.Platform.UI.WPF.Shell.DI;
using DoubleGis.Platform.UI.WPF.Shell.Settings;

using Microsoft.Practices.Unity;

using Nuclear.Tracing.API;

namespace DoubleGis.Platform.UI.WPF.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IUnityContainer _container = new UnityContainer();
        private readonly IShellSettings _shellSettings;
        private readonly ITracer _tracer;

        public App(ITracer tracer)
        {
            if (tracer == null)
            {
                throw new ArgumentNullException("tracer");
            }

            _tracer = tracer;

            _shellSettings = new ShellSettings();
            _container.RegisterInstance<IShellSettings>(_shellSettings, Lifetime.Singleton);
            
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var currentThread = Thread.CurrentThread;
            currentThread.Name = currentThread.Name ?? "GUI";

            currentThread.CurrentCulture = _shellSettings.TargetCulture;
            currentThread.CurrentUICulture = _shellSettings.TargetCulture;
            CultureInfo.DefaultThreadCurrentCulture = _shellSettings.TargetCulture;

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            _container
                .ConfigureDI(_tracer)
                .Run(_tracer);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var container = _container;
            if (container != null)
            {
                container.Dispose();
            }

            DispatcherUnhandledException -= OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }
        
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var targetName = new AssemblyName(args.Name);
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.FullName == targetName.FullName);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = _tracer;
            if (logger != null)
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    logger.FatalFormat(ex, "UnhandledException. IsTerminating={0}.", e.IsTerminating);
                }
                else
                {
                    logger.FatalFormat("UnhandledException. IsTerminating={0}. Description: {1}", e.IsTerminating, e.ExceptionObject);
                }
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = _tracer;
            if (logger != null)
            {
                logger.ErrorFormat("Dispatcher unhanlded exception catched. Is handled: {0}. Exception: {1}", e.Handled, e.Exception);
            }
        }
    }
}
