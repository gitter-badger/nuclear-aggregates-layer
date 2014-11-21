namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules
{
    /// <summary>
    /// ��������� ������, ������� ������ �������� ������� ������, ��������� �����-�� background ������
    /// </summary>
    public interface IStandaloneWorkerModule : IModule
    {
        void Run();
        void TryStop();
        void Wait();
    }
}