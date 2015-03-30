using System;
using System.IO;
using System.Linq;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Extensions;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Documents;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Navigation;
using DoubleGis.Platform.UI.WPF.Shell.Presentation.Blendability;
using DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell;

using Microsoft.Practices.Unity;

using NuClear.Tracing.API;
using NuClear.Tracing.Log4Net.Config;

namespace DoubleGis.Platform.UI.WPF.Shell.DI
{
    public static partial class Bootstrapper
    {
        public static IShellViewModel DesignTimeResolveShellViewModel()
        {
            ITracer tracer = null;
            try
            {
                #region Описание особенностей работы VS designer
                // проблема - VS designer (XDesProc.exe) хитро загружает сборку контрола в свой процесс:
                // - копирует в спец temp директорию вида AppData\Local\Microsoft\VisualStudio\11.0\Designer\ShadowCache\tf1n0ckv.cca\afuzcwaq.3cr саму сборку контрола, а также её зависимости (на которые есть reference)
                // - загружает сборки в AppDomain, видимо через Assembly.LoadFile, т.о. сборка оказывается в так называемом "no context" пространстве, то же происходит и с её зависимостями
                // Из-за загрузки в "no context" сборки в который выполняется код resolve Design time view model (сборки shell), код продолжая выполнение - начинает resolve модулей
                // Если загружать сборки модулей и их зависимости также как и в runtime, будут проблемы - т.к. в runtime сборки модулей подгружаются через Assembly.Load в default load context, 
                // т.о. при переборе реализуемых интерфейсов оказывается, что typeof(IModule) != typeof(IModule) (один IModule из одного load context, второй из другого)
                // Чтобы обойти эту проблему, все сборки модулей и их зависимости в тот же самый "no context", в который VS designer загрузил сборку контрола shellview
                // Кроме явной загрузки сборок из директории модулей, нужно учитывать возможность опережающей загрузки зависимостей самой CLR - это достигается благодаря подписке на событие AppDomain.CurrentDomain.AssemblyResolve
                // ИТОГО - все сборки загружаются сразу в нужный контекст, и код boostrapper также вызывается в сборке загруженной в нужны контекст, поэтому при resolve модулей проблем не возникает
                // ЗАМЕЧАНИЕ: сборки модулей загружаются из директории в которой они находятся после build, поэтому после загрузки их в процесс XDesProc.exe, файлы этих сборок блокируются =>
                //  последующий build не пройдет, чтобы этого избежать при запуске нового build принудительно завершается процесс XDesProc.exe если он был запущен, через PreBuildEvent всех WPF проектов
                // Более цивилизованные варианты - подключиться к API автоматизации Visual studio (см. EnvDTE) и провоцировать перезагрузку/перерисовку Designer 
                #endregion

                tracer = Log4NetTracerBuilder.Use
                                             .Trace
                                             .File("Erm.WPF.Client.DesignTime")
                                             .Build;

                tracer.Info("Design time configuring started ...");

                DesignTimeAssemblyLoader.Attach(tracer);

                var container = new UnityContainer();
                LocalPath = DesignTimePaths.LocalPath;

                var queryableContainerExtension = new QueryableContainerExtension();
                container.AddExtension(queryableContainerExtension);
                container.RegisterInstance(Mapping.QueryableExtension, queryableContainerExtension);

                container.RegisterInstance<ITracer>(tracer, Lifetime.Singleton)
                         ////.RegisterModules(DesignTimeAssemblyLoader.AssemblyLoaderToNoLoadContext)
                         .RegisterModules()
                         .DesignTimeConfigureModules()
                         .ConfigureShellInfrastructure()
                         .ConfigureDispatcher()
                         .RegisterType<IDocumentManager, DocumentManager>(Lifetime.Singleton)
                         .RegisterType<IDocumentsStateInfo, DocumentManager>(Lifetime.Singleton)
                         .RegisterType<INavigationManager, NavigationManager>(Lifetime.Singleton);

                var standaloneWorkerModules = container.ResolveAll<IStandaloneWorkerModule>().Where(m => m is IDesignTimeStandaloneWorkerModule);
                foreach (var module in standaloneWorkerModules)
                {
                    ((IDesignTimeStandaloneWorkerModule)module).Run();
                }

                var shellViewModel = container.Resolve<IShellViewModel>();
                tracer.Info("Design time shell view model successfully resolved");

                return shellViewModel;

            }
            catch (FileNotFoundException ex)
            {
                if (tracer != null)
                {
                    tracer.Fatal(ex, "Can't load required file. " + ex.FileName + ". " + ex.FusionLog);
                    tracer.Fatal(ex, "Can't resolve design time view model");
                }
                throw;
            }
            catch (Exception ex)
            {
                if (tracer != null)
                {
                    tracer.Fatal(ex, "Can't resolve design time view model");
                }
                throw;
            }
            finally
            {
                //DesignTimeAssemblyLoader.Deattach();
            }
        }

        private static IUnityContainer DesignTimeConfigureModules(this IUnityContainer container)
        {
            var modulesContainers = container.ResolveAll<IModulesContainer>().Where(mc => mc is IDesignTimeModuleContainer);
            var modules = container.ResolveAll<IModule>().Where(mc => mc is IDesignTimeModule);

            foreach (var modulesContainer in modulesContainers)
            {
                ((IDesignTimeModuleContainer)modulesContainer).Configure();
            }

            foreach (var module in modules)
            {
                ((IDesignTimeModule)module).Configure();
            }

            return container;
        } 
    }
}
