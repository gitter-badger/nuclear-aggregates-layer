namespace DoubleGis.Erm.Platform.API.Core.UseCases
{
    /// <summary>
    /// Позволяет влиять на поведение (через настройки) usecase в процессе его исполнения
    /// Т.е. непосредственно в потоке ControlFlow, изменяем его настройки
    /// </summary>
    public interface IUseCaseTuner
    {
        void AlterDuration<THost>() where THost : class;
    }
}
