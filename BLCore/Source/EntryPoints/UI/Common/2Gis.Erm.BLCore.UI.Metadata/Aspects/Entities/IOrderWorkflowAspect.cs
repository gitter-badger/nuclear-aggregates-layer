using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IOrderWorkflowAspect : IAspect
    {
        // TODO {all, 23.01.2015}: Избавиться от 'Id' одновременно с Order
        OrderState WorkflowStepId { get; }
    }
}
