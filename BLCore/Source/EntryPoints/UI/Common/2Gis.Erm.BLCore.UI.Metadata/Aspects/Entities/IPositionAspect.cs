namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IPositionAspect : IAspect
    {
        string PositionName { get; }
        long PositionId { get; }
    }
}