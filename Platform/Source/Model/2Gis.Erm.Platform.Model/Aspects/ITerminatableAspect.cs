namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface ITerminatableAspect : IAspect
    {
        bool IsTerminated { get; }
    }
}