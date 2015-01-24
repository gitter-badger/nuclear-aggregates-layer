namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects
{
    public interface IDeactivatableAspect : IAspect
    {
        bool IsActive { get; }
    }
}