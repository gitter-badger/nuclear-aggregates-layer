namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects
{
    public interface INewableAspect : IAspect
    {
        bool IsNew { get; }
    }
}