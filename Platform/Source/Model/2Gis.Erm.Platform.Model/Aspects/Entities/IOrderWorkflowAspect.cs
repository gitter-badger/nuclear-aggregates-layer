using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IOrderWorkflowAspect : IAspect
    {
        OrderState WorkflowStepId { get; }
        int PreviousWorkflowStepId { get; set; }
        string AvailableSteps { get; set; }
    }
}
