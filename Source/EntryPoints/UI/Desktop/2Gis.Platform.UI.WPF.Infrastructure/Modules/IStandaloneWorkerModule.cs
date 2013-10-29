namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules
{
    /// <summary>
    /// Интерфейс модуля, который помимо основных функций модуля, выполняет какие-то background задачи
    /// </summary>
    public interface IStandaloneWorkerModule : IModule
    {
        void Run();
        void TryStop();
        void Wait();
    }
}