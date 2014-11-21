using System.Threading.Tasks;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer
{
    public interface IPerformedOperationsConsumer
    {
        Task Consume();
    }
}