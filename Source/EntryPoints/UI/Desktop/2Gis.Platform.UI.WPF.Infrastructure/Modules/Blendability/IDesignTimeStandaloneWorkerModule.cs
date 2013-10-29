namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability
{
    public interface IDesignTimeStandaloneWorkerModule : IDesignTimeModule
    {
        void Run();
        void TryStop();
        void Wait();
    }
}
