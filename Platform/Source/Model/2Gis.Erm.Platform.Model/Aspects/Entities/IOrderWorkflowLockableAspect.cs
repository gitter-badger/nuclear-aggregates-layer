namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IOrderWorkflowLockableAspect : IAspect
    {
        bool IsWorkflowLocked { get; set; }
    }
}