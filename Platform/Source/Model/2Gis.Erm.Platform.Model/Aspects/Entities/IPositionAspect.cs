namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IPositionAspect : IAspect
    {
        string PositionName { get; }
        long? PositionId { get; }
    }
}