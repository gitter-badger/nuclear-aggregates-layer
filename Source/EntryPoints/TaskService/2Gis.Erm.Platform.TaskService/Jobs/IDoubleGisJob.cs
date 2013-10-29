using Quartz;

namespace DoubleGis.Erm.Platform.TaskService.Jobs
{
    /// <summary>
    /// Маркерный интерфейс - используется при конфигурировании DI. 
    /// Расширяет интерфейс IJob, т.к. иначе Quartz не может вызвать нашу job-фабрику - кидаются ошибки, что job должен реализовывать интерфейс IJob
    /// </summary>
    public interface IDoubleGisJob : IJob
    {
    }
}