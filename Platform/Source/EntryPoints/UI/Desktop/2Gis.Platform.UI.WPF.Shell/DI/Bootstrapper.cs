using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Extensions;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Manager;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util.Threading;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Documents;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Navigation;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Notifications;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Toolbar;
using DoubleGis.Platform.UI.WPF.Shell.Layout.UserInfo;
using DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell;

using Microsoft.Practices.Unity;

using Nuclear.Settings.API;
using Nuclear.Tracing.API;

namespace DoubleGis.Platform.UI.WPF.Shell.DI
{
    public static partial class Bootstrapper
    {
        private const string ModulesRelativePath = "Modules";

        public static string GetApplicationWorkingDirectory
        {
            get
            {
                var domain = AppDomain.CurrentDomain;
                return string.IsNullOrEmpty(domain.RelativeSearchPath)
                           ? domain.BaseDirectory
                           : domain.RelativeSearchPath;
            }
        }

        private static string LocalPath { get; set; }

        private static string ModulesRootDirectoryPath
        {
            get
            {
                if (!string.IsNullOrEmpty(LocalPath))
                {
                    var modulesPath = Path.Combine(LocalPath, ModulesRelativePath);
                    if (Directory.Exists(modulesPath))
                    {
                        return modulesPath;
                    }
                }

                return null;
            }
        }

        public static IUnityContainer ConfigureDI(this IUnityContainer container, ITracer tracer)
        {
            LocalPath = GetApplicationWorkingDirectory;

            tracer.Info("Start configure. Startup directory: " + LocalPath);

            try
            {
                var queryableContainerExtension = new QueryableContainerExtension();
                container.AddExtension(queryableContainerExtension);
                container.RegisterInstance(Mapping.QueryableExtension, queryableContainerExtension);

                container.RegisterLogger(tracer)
                         .RegisterModules()
                         .RegisterType<IDocumentManager, DocumentManager>(Lifetime.Singleton)
                         .RegisterType<IDocumentsStateInfo, DocumentManager>(Lifetime.Singleton)
                         .RegisterType<INavigationManager, NavigationManager>(Lifetime.Singleton)
                         .RegisterType<IToolbarManager, ToolbarManager>(Lifetime.Singleton)
                         .ConfigureModules()
                         .ConfigureShellInfrastructure()
                         .ConfigureDispatcher();
            }
            catch (Exception ex)
            {
                tracer.FatalFormat(ex, "Can't configure application");
                throw;
            }

            tracer.Info("Configured successfully");

            return container;
        }

        public static IUnityContainer Run(this IUnityContainer container, ITracer tracer)
        {
            try
            {
                tracer.Info("Start running ...");
                var standaloneWorkerModules = container.ResolveAll<IStandaloneWorkerModule>();
                foreach (var module in standaloneWorkerModules)
                {
                    module.Run();
                }

                var shellViewModel = container.Resolve<IShellViewModel>();
                var shellView = new ShellWindow { DataContext = shellViewModel };
                shellView.Show();
            }
            catch (Exception ex)
            {
                tracer.ErrorFormat(ex, "Can't run application");
                throw;
            }

            tracer.Info("Run successfully ...");

            return container;
        }

        private static IUnityContainer ConfigureShellInfrastructure(this IUnityContainer container)
        {
            return container
                    .RegisterType<IDocumentManagerViewModel, DocumentManager>(Lifetime.Singleton)
                    .RegisterType<INavigationManagerViewModel, NavigationManager>(Lifetime.Singleton)
                    .RegisterType<IUserManagerViewModel, UserManager>(Lifetime.Singleton)
                    .RegisterType<INotificationsManagerViewModel, NotificationsManager>(Lifetime.Singleton)
                    .RegisterType<IToolbarManagerViewModel, ToolbarManager>(Lifetime.Singleton)
                    .RegisterType<IShellViewModel, ShellViewModel>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureDispatcher(this IUnityContainer container)
        {
            return container.RegisterType<IUIDispatcher, UIDispatcher>(Lifetime.Singleton, new InjectionConstructor(Dispatcher.CurrentDispatcher));
        }

        private static IUnityContainer RegisterLogger(this IUnityContainer container, ITracer tracer)
        {
            return container.RegisterInstance<ITracer>(tracer, Lifetime.Singleton);
        }

        private static IUnityContainer RegisterModules(this IUnityContainer container)
            {
            var modulesDirectoryPath = ModulesRootDirectoryPath;
            if (string.IsNullOrEmpty(modulesDirectoryPath))
            {
                return container;
            }

            var logger = container.Resolve<ITracer>();

            var containersUniqueMap = new HashSet<Guid>();
            var modulesUniqueMap = new HashSet<Guid>();

            var assemblyManager = new ModulesFinder(modulesDirectoryPath, "*.dll", SearchOption.AllDirectories);
            var modulesFinder = new ModulesManager(assemblyManager);
            foreach (var modulesDescriptor in modulesFinder.FindModules())
            {
                try
                {
                    modulesFinder.LoadModule(modulesDescriptor);
                    RegisterSettings(container, modulesDescriptor.ModulesContainerConfigFullPath, modulesDescriptor.Settings);
                }
                catch (Exception ex)
                {
                    logger.FatalFormat(ex, "Can't process settings for module container with path {0}", modulesDescriptor.ModulesContainerFullPath);
                    throw;
                }

                IModulesContainer moduleContainer;
                try
                {
                    moduleContainer = (IModulesContainer)container.Resolve(Type.GetType(modulesDescriptor.ContainerType));
                }
                catch (Exception ex)
                {
                    logger.FatalFormat(ex,
                                            "Can't create module container of type {0} from file {1}",
                                            modulesDescriptor.ContainerType,
                                            modulesDescriptor.ModulesContainerFullPath);
                    throw;
                }

                if (!containersUniqueMap.Add(moduleContainer.Id))
                {
                    var msg = string.Format("Module container of type {0} has duplicated Id: {1}",
                                            modulesDescriptor.ContainerType,
                                            moduleContainer.Id);
                    logger.Fatal(msg);
                    throw new InvalidOperationException(msg);
                }

                container.RegisterInstance(ModuleIndicators.Container, moduleContainer.Id.ToString(), moduleContainer);
                foreach (var moduleType in modulesDescriptor.ModuleTypes)
                {
                    IModule module;
                    try
                    {
                        module = (IModule)container.Resolve(Type.GetType(moduleType));
                    }
                    catch (Exception ex)
                    {
                        logger.FatalFormat(ex, "Can't create module of type {0} from file {1}", moduleType, modulesDescriptor.ModulesContainerFullPath);
                        throw;
                    }

                    if (!modulesUniqueMap.Add(module.Id))
                    {
                        var msg = string.Format("Module of type {0} has duplicated Id: {1}", moduleType, module.Id);
                        logger.Fatal(msg);
                        throw new InvalidOperationException(msg);
                    }

                    container.RegisterInstance(ModuleIndicators.Module, module.Id.ToString(), module);
                    if (module is IStandaloneWorkerModule)
                    {
                        container.RegisterInstance(ModuleIndicators.StandaloneWorkerModule, module.Id.ToString(), module);
                    }
                }
            }

            return container;
        }

        private static IUnityContainer ConfigureModules(this IUnityContainer container)
        {
            var modulesContainers = container.ResolveAll<IModulesContainer>();
            var modules = container.ResolveAll<IModule>();

            foreach (var modulesContainer in modulesContainers)
            {
                modulesContainer.Configure();
            }

            foreach (var module in modules)
            {
                module.Configure();
            }

            return container;
        }

        private static void RegisterSettings(IUnityContainer container, string configFileFullPath, IEnumerable<SettingsContainerDescriptor> descriptors)
        {
            foreach (var settingContainerDescriptor in descriptors)
            {
                var settingContainer = (ISettingsContainer)container.Resolve(Type.GetType(settingContainerDescriptor.Implementation), new DependencyOverrides { { typeof(string), configFileFullPath } });
                container.ConfigureSettingsAspects(settingContainer);
            }
        }
    }
}
