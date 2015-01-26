namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface IDeactivatableAspect : IAspect
    {
        bool IsActive { get; }
    }
}