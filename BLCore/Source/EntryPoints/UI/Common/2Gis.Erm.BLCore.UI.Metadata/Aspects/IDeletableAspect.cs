namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects
{
    public interface IDeletableAspect : IAspect
    {
        bool IsDeleted { get; }
    }
}